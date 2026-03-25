using ExpressYourself.Domain.ValueObjects;

namespace ExpressYourself.Domain.Entities;

/// <summary>
/// Represents an IP address entry stored in the system.
/// </summary>
public sealed class IpAddress
{
    public int Id { get; private set; }
    public int CountryId { get; private set; }
    public IpAddressValue Address { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Required by EF Core.
    /// </summary>
    private IpAddress()
    {
        Address = null!;
    }

    /// <summary>
    /// Creates a new IpAddress instance.
    /// </summary>
    /// <param name="countryId">Country identification(required).</param>
    /// <param name="address">IP address string.</param>
    public IpAddress(int countryId, IpAddressValue address)
    {
        CountryId = countryId;
        Address = address;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCountry(int countryId)
    {
        if (CountryId == countryId)
        {
            return;
        }

        CountryId = countryId;
        UpdatedAt = DateTime.UtcNow;
    }
}