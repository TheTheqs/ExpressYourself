using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ExpressYourself.services;
// This class centralizes all database operations, serving as the only layer interacting with the Entity Framework.
// As a static class, it has no attributes, only methods representing all CRUD operations for each table.
// The framework's access will always be done through punctual instantiation of the DbContext.
// All CRUD functions are asynchronous to ensure better performance, considering the potential high volume of requests. 
// These methods return a boolean to communicate the success or failure of the operations.
public static class DB { //DB stands for DATABASE!!
    //C- Create
    public static async Task<bool> SaveCountry(Country newCountry) //Country
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                await context.Countries.AddAsync(newCountry);
                await context.SaveChangesAsync();
                return true;
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return false;
    }
        public static async Task<bool> SaveIP(Ipaddress newIP) //IP
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                await context.Ipaddresses.AddAsync(newIP);
                await context.SaveChangesAsync();
                return true;
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
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
                .SingleOrDefaultAsync(c => c.TwoLetterCode == twoLetterCode);
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
                .SingleOrDefaultAsync(i => i.Ip == ip);
                
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
                    ipAddress.UpdatedAt = DateTime.UtcNow;
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

    //D- Destroy - This function is not meant to be used, since there is no destruction of data from the 
    //database in the current application scope. But it's important to have it for a possible further usage.
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
    
    //Util functions
    //Get Country ID - This function returns A country id or create a new country in the database if does not exist and return its id.
    public static async Task<int> GetCountryId(Country nCountry)
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                var country = await GetCountryByCode(nCountry.TwoLetterCode);
                if(country == null)
                {
                    await SaveCountry(nCountry);
                    country = await GetCountryByCode(nCountry.TwoLetterCode);
                };
                if(country != null)
                {
                    return country.Id;
                };
                return 0; //Necessary to remove the warning
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return 0;
    }

    //Get Ip ID - This function returns a ip id or create a new ip in the database if does not exist and return its id.
    public static async Task<int> GetIpId(Ipaddress nIpaddress)
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                var nIp = await GetIpaddress(nIpaddress.Ip);
                if(nIp == null)
                {
                    await SaveIP(nIpaddress);
                    nIp = await GetIpaddress(nIpaddress.Ip);
                };
                if(nIp != null)
                {
                    return nIp.Id;
                };
                return 0; //Necessary to remove the warning
            };
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return 0;
    }
};