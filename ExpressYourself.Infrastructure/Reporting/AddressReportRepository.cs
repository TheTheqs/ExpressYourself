using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.UseCases.GetAddressReport;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ExpressYourself.Infrastructure.Reporting;

/// <summary>
/// Provides raw SQL access for address report data.
/// </summary>
public sealed class AddressReportRepository : IAddressReportRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressReportRepository"/> class.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    public AddressReportRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Retrieves the address report grouped by country.
    /// When country codes are provided, only matching countries are included.
    /// When the filter is null or empty, all countries are included.
    /// </summary>
    /// <param name="twoLetterCodes">Optional list of two-letter country codes used to filter the report.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection containing the report rows.</returns>
    public async Task<IReadOnlyCollection<GetAddressReportItemResponse>> GetReportAsync(
        IReadOnlyCollection<string>? twoLetterCodes,
        CancellationToken cancellationToken = default)
    {
        var results = new List<GetAddressReportItemResponse>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = BuildSql(twoLetterCodes, command);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new GetAddressReportItemResponse
            {
                CountryName = reader.GetString(reader.GetOrdinal("CountryName")),
                AddressesCount = reader.GetInt32(reader.GetOrdinal("AddressesCount")),
                LastAddressUpdated = reader.GetDateTime(reader.GetOrdinal("LastAddressUpdated"))
            });
        }

        return results;
    }

    /// <summary>
    /// Builds the raw SQL statement and appends any required parameters.
    /// </summary>
    /// <param name="twoLetterCodes">Optional list of two-letter country codes used to filter the report.</param>
    /// <param name="command">The SQL command that receives the generated parameters.</param>
    /// <returns>The SQL statement to execute.</returns>
    private static string BuildSql(
        IReadOnlyCollection<string>? twoLetterCodes,
        SqlCommand command)
    {
        var sql = """
                  SELECT
                      c.Name AS CountryName,
                      COUNT(ip.Id) AS AddressesCount,
                      MAX(ip.UpdatedAt) AS LastAddressUpdated
                  FROM Countries c
                  INNER JOIN IPAddresses ip ON ip.CountryId = c.Id
                  """;

        if (twoLetterCodes is not null && twoLetterCodes.Count > 0)
        {
            var parameterNames = new List<string>();
            var index = 0;

            foreach (var code in twoLetterCodes)
            {
                var parameterName = $"@Code{index++}";
                parameterNames.Add(parameterName);

                command.Parameters.Add(new SqlParameter(parameterName, SqlDbType.Char, 2)
                {
                    Value = code
                });
            }

            sql += $"""
                    
                    WHERE c.TwoLetterCode IN ({string.Join(", ", parameterNames)})
                    """;
        }

        sql += """
                
                GROUP BY c.Name
                ORDER BY c.Name;
                """;

        return sql;
    }
}