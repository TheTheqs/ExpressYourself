using System.Runtime.InteropServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;
using Npgsql.Replication;
namespace ExpressYourself.services;

public static class CM //Stands for Cache Memory!
{
    private static IMemoryCache? cache; //Static declaration of the cache attribute. = null! is for warning removing.
    private static readonly MemoryCacheEntryOptions DefaultCacheOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1), // Configure the TTL to 
        SlidingExpiration = TimeSpan.FromHours(1)// Reset the TTL after access.
    };

    //Configuration, used in instantiate.
    public static void Configure(IMemoryCache memoryCache)  
    {
        cache = memoryCache;
    }

    //Get obj function. It will return null if the reference don't exist.
    public static async Task<Ipaddress?> Get<T>(String key)
    {
        try
        {
            if(cache != null && cache.TryGetValue(key, out Ipaddress? value)) //? for warning removing.
            {
                Console.WriteLine($"[Request] The IP {key} was successefully requested from the CACHE");
                return  await Task.FromResult(value);
            }
            return null;
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return default;
    }
    //Add obj function.
    public static async Task<bool> Create<T, Ipaddress> (String key, Ipaddress value)
    {
        try
        {
            if(cache != null)
            {
                if(cache.TryGetValue(key, out _))
                {
                    return false; //Key already exist. This context is never suposed to exist.
                }
                cache.Set(key, value, DefaultCacheOptions);
                return await Task.FromResult(true); //Success
            }
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return false;
    }
    //Update obj function.
    public static async Task<bool> Update<Ipaddress> (String key, Ipaddress value)
    {
        try
        {
            Console.WriteLine("Trying to update value in cache...");
            if(cache != null)
            {
                if(!cache.TryGetValue(key, out _))
                {
                    return false; //Key does not exist. This context is also suposed to never happen.
                }
                cache.Set(key, value, DefaultCacheOptions);
                Console.WriteLine("Succes Update Value");
                return await Task.FromResult(true); //Success
            }
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        Console.WriteLine("Failed to update cache value");
        return false;

    }
    //Delete function - Used when an IP reference is no longer.
       public static async Task<bool> Delete<String> (String key)
    {
        Console.WriteLine("Trying to delete value from cache...");
        try
        {
            if(cache != null && key != null)
            {
                if(!cache.TryGetValue(key, out _))
                {
                    return false; //Key does not existe. This context is suposed to never happen.
                }
                cache.Remove(key);
                Console.WriteLine("Successfully deleted value");
                return await Task.FromResult(true); //Success
            }
        }
        catch(Exception err)
        {
            Console.WriteLine("Failed to delete value from cache");
            Console.WriteLine(err.Message);
        }
        return false;
    }
    //Verify if the key exist. Will be used in the upgrade system
    public static async Task<bool> HasKey(String key)
    {
        if(cache!= null)
        {
            return await Task.FromResult(cache.TryGetValue(key, out _));
        }
        return false;
    }
}