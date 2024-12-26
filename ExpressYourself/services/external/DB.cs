using System.Data;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

namespace ExpressYourself.services;
// This class centralizes all database operations, serving as the only layer interacting with the Entity Framework and PostGree.
// As a static class, it has no attributes, only methods representing all CRUD operations for each table and some Util functionalities.
// The framework's access will always be done through punctual instantiation of the DbContext.
// All CRUD functions are asynchronous to ensure better performance, considering the potential high volume of requests. 
// These methods return a boolean to communicate the success or failure of the operations.
public static class DB { //DB stands for DATABASE!!
    //The following static attribute and function are necessary for the third task, which need the application of raw sql.
    private static string? _connectionString;

    public static void Initialize(string? connectionString)
    {
        _connectionString = connectionString;
    }
    //C- Create
    public static async Task<Country?> SaveCountry(Country newCountry) //Country. Return Country for better performance ai EA class.
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                Console.WriteLine($"[DataBase]Trying to save country {newCountry.Name}");
                await context.Countries.AddAsync(newCountry);
                await context.SaveChangesAsync();
                Console.WriteLine($"[DataBase]Country saved in th Database {newCountry.Name}");
                return newCountry;
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
            if(err.InnerException != null)
            {
            Console.WriteLine(err.InnerException.Message);
            }
        }
        return null;
    }
        public static async Task<bool> SaveIP(Ipaddress cloneIP) //IP
    {
        try
        {
            Ipaddress newIP = new Ipaddress(cloneIP); //by that, you garantee that Country is null, this is necessary to avoid save it too.
            using (var context = new IpLocatorContext())
            {
                Console.WriteLine($"[DataBase]Tryng to save IP {newIP.Ip} in DataBase");
                await context.Ipaddresses.AddAsync(newIP);
                await context.SaveChangesAsync();
                return true;
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
            if(err.InnerException != null)
            {
            Console.WriteLine(err.InnerException.Message);
            }
        }
        return false;

    }

    //R - Read
    // the "?" is necessary to avoid warnings.
    public static async Task<Country?> GetCountryByCode(String twoLetterCode) //Country
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                return await context.Countries
                .FirstOrDefaultAsync(c => c.TwoLetterCode == twoLetterCode);
            };
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return null;
    }

    public static async Task<Ipaddress?> GetIpaddress(String ip) //IP
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                return await context.Ipaddresses
                .Include(c => c.Country) //Necessary line to include the entire Country Object in the instance
                .FirstOrDefaultAsync(i => i.Ip == ip);
                
            };
        }
        catch (Exception err)
        {
           Console.WriteLine(err.Message); 
        }
        return null;
    }

    //U - Update - Only for Ipaddress
    public static async Task<bool> UpdateIP(Ipaddress uIP) //IP
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                var ipAddress = await context.Ipaddresses
                .SingleOrDefaultAsync(i => i.Ip == uIP.Ip);
                if(ipAddress != null)
                {
                    ipAddress.CountryId = uIP.CountryId;
                    ipAddress.UpdatedAt = GetCurrentTime();
                    await context.SaveChangesAsync();
                    return true;
                };
                return false;
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return false;
    }

    //D- Destroy - This function is meant to be used only when an IP in the data is no longe ascessible by the External API.
    public static async Task<bool> DeleteCountry(Country oCountry) //Country
    { 
        try
        {
            using (var context = new IpLocatorContext())
            {
                var country = await context.Countries.FindAsync(oCountry.Id);
                if(country != null)
                {
                    context.Countries.Remove(country);
                    context.SaveChanges();
                    return true;
                };
                return false;
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return false;
    }
        public static async Task<bool> DeleteIP(Ipaddress oIpAddress) //IP
    { 
        try
        {
            using (var context = new IpLocatorContext())
            {
                var ipAddress = await context.Ipaddresses.FirstOrDefaultAsync(i => i.Ip == oIpAddress.Ip);
                if(ipAddress != null)
                {
                    context.Ipaddresses.Remove(ipAddress);
                    context.SaveChanges();
                    return true;
                };
                return false;
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return false;
    }

    //Util
    //The following function is used to check if a country already exist in the database
    public static async Task<bool> HasCountry(String code)
    {
        try
        {
            using(var context = new IpLocatorContext())
            {
                return await context.Countries.AnyAsync(c => c.TwoLetterCode == code);
            }
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return false;
    }
    //The following function generates a Date variable without timezone for DataBase compatibility.
    public static DateTime GetCurrentTime()
    {
        DateTime compatibleDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified); // get a date without timezone
        return compatibleDateTime;
    }
    //The following function return the number of IPs in the DataBase
    public static async Task<int> GetIpCount()
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                return await context.Ipaddresses.CountAsync();
            }
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return -1; //Return a non valid value in case of error. 0 could cause some trouble in the Update Class.
    }
    //The following function is used at the update function
    public static async Task<List<Ipaddress>> GetIpaddressesPack(int startIndex, int endIndex)
    {
        try
        {
            if (startIndex < 0 || endIndex < startIndex)
            {
                throw new ArgumentException("Invalid range specified"); //To avoid any possible error
            }
            int count = endIndex - startIndex + 1; //number of itens in the list (suposed to be always 100)
            using (var context = new IpLocatorContext())
            {
                return await context.Ipaddresses
                .OrderBy(x => x.Id) //ordering by Id
                .Skip(startIndex) //Starting from the given position
                .Take(count) //get equal or less the count number
                .ToListAsync(); //convert into a list, given in the response.
            }
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        Console.WriteLine("Failed at getting packed IP for the updating function");
        return new List<Ipaddress>(); // Return an empty list on failure
    }

    //Below you will find all database interactions for the Third task.
    public static async Task<DataTable?> GetAllReports()
    {
        var dataTable = new DataTable(); //empty returnable object
        try
        {
            String query = @" 
                SELECT 
                    ""Countries"".""Name"" AS ""CountryName"",
                    COUNT(""IPAddresses"".""Id"") AS ""AddressesCount"",
                    MAX(""IPAddresses"".""UpdatedAt"") AS ""LastAddressUpdate""
                FROM 
                    ""Countries""
                INNER JOIN 
                    ""IPAddresses"" ON ""Countries"".""Id"" = ""IPAddresses"".""CountryId""
                GROUP BY 
                    ""Countries"".""Name""
                ORDER BY 
                    ""Countries"".""Name"";
                "; //The actual Query that provides all countries from the database.

            await using var connection = new NpgsqlConnection(_connectionString); //create a connection and open it.
            await connection.OpenAsync();
            await using var queryCommand = new NpgsqlCommand(query, connection); //build the commando object
            await using var response = await queryCommand.ExecuteReaderAsync(); //create a response
            dataTable.Load(response); //try to atatch the attribute to the Data Table

            return dataTable;
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return dataTable;        
    }
    public static async Task<(DataTable?, String?)> GetSelectedReports(List<String> countryCodes) //Need to return DataTable and String
    {
        var dataTable = new DataTable(); //empty returnable object
        List<String>? invalidCodes = await ValidateCodes(countryCodes);//code validation
        countryCodes.RemoveAll(invalidCodes.Contains);
        if(countryCodes.Count() > 0)
        {
            try
            {
                String query = @"
                SELECT 
                    ""Countries"".""Name"" AS ""CountryName"",
                    COUNT(""IPAddresses"".""Id"") AS ""AddressesCount"",
                    MAX(""IPAddresses"".""UpdatedAt"") AS ""LastAddressUpdated""
                FROM 
                    ""Countries""
                INNER JOIN 
                    ""IPAddresses"" ON ""Countries"".""Id"" = ""IPAddresses"".""CountryId""
                WHERE ""Countries"".""TwoLetterCode"" IN (" + string.Join(",", countryCodes.Select((_, i) => $"@p{i}")) + @")
                GROUP BY ""Countries"".""Name""
                ORDER BY ""Countries"".""Name"";
                "; //This Query provides filtered countries from the database.
                await using var connection = new NpgsqlConnection(_connectionString); //create a connection and open it.
                await connection.OpenAsync();

                await using var queryCommand = new NpgsqlCommand(query, connection); //build the command object

                for (int i = 0; i < countryCodes.Count; i++)
                {
                    queryCommand.Parameters.AddWithValue($"@p{i}", countryCodes[i]); //add the parameters
                }

                await using var response = await queryCommand.ExecuteReaderAsync(); //create a response
                dataTable.Load(response); //try to atatch the attribute to the Data Table
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }           
        }
        //adding a warning in case there was invalid provided data
        string invalidMessage = invalidCodes.Count > 0 
        ? string.Join(", ", invalidCodes)
        : string.Empty;
        return (dataTable, invalidMessage); //Return seccessfull request
    }

    //The following function will filter the provided Country List and will return an array with codes that has no data.
    //This could be done with EF, but to keep accurate to the task requisistion we will use raw SQL
    private static async Task<List<string>> ValidateCodes(List<string> countryCodes)
    {
        // Lista para armazenar os códigos inválidos.
        var invalidCodes = new List<string>();

        try
        {
            foreach(String codezin in countryCodes)
            {
                if(await IsItemInDatabase(codezin) == false)
                {
                    invalidCodes.Add(codezin); //add code in invalid list if it is not in database.
                }
            }
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return invalidCodes; //Return an filled or empty list
    }
    //Modular Function for key validation
    private static async Task<bool> IsItemInDatabase(string item)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(item) || item.Length > 2)//Verify if item isn't null or empty
            {
                return false;
            }

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(); //Unfortunatelly I was not able to reuse the already openned conections :(

            string query = $@" 
                SELECT COUNT(1)
                FROM ""Countries""
                WHERE ""TwoLetterCode"" = @p1;
            "; //SQL command

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@p1", item); //getting parameter

            var result = await command.ExecuteScalarAsync();//The actual validation

            return Convert.ToInt32(result) > 0; //If the returned value is > 0, return true (the item is in the database). Else return false.
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return false; //Just in case
        }
    }
};