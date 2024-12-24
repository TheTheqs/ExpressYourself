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
    public static async Task<Country?> SaveCountry(Country newCountry) //Country. Return Country for better performance ai EA class.
    {
        try
        {
            using (var context = new IpLocatorContext())
            {
                Console.WriteLine("Trying to save country");
                await context.Countries.AddAsync(newCountry);
                await context.SaveChangesAsync();
                Console.WriteLine("Country saved in th Database");
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
                Console.WriteLine("Tryng to save IP");
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
            Console.WriteLine(err.InnerException.Message + " This ERROR");
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
                Console.WriteLine("Getting from DB!");
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

    //Util, the following class is used to check if a country already exist in the database
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
};