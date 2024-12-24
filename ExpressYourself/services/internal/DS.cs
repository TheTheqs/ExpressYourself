using System.Text.Json;

namespace ExpressYourself.services;

//This class is responsible for give serialized data as response in the API.
//The functions will be async for a better performance.
public static class DS { //Stands for Data Serializer!!
    
    //Get ip origin without the IP
    public static async Task<object> GetCountry(Ipaddress ipaddress)
    {
        try
        {
            var jsoned = new
            {
                CountryName = ipaddress.Country.Name, //CountryName attribution for better display
                ipaddress.Country.TwoLetterCode,
                ipaddress.Country.ThreeLetterCode
            };

        return await Task.Run(() => jsoned);
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return "Error: Could not parse object to JSON!";
    }
    //Get ip origin with the IP
    public static async Task<object> GetIpCountry(Ipaddress ipaddress)
    {
        try
        {
            var jsoned = new
            {
                ipaddress.Ip,
                CountryName = ipaddress.Country.Name, //CountryName attribution for better display
                ipaddress.Country.TwoLetterCode,
                ipaddress.Country.ThreeLetterCode
            };

        return await Task.Run(() => jsoned);
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return "Error: Could not parse object to JSON!";
    }
}
