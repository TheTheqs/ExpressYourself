using System.Text.Json;

namespace ExpressYourself.services;

//This class is responsible for give serialized data as response in the API.
//The functions will be async for a better performance.
public static class DS { //Stands for Data Serializer!!
    
    //Get ip origin without the IP
    public static async Task<String> GetCountry(Ipaddress ipaddress)
    {
        var jsoned = new
        {
            ipaddress.Country.Name,
            ipaddress.Country.TwoLetterCode,
            ipaddress.Country.ThreeLetterCode
        };

        return await Task.Run(() => JsonSerializer.Serialize(jsoned));
    }
    //Get ip origin with the IP
    public static async Task<String> GetIpCountry(Ipaddress ipaddress)
    {
        var jsoned = new
        {
            ipaddress.Ip,
            ipaddress.Country.Name,
            ipaddress.Country.TwoLetterCode,
            ipaddress.Country.ThreeLetterCode
        };

        return await Task.Run(() => JsonSerializer.Serialize(jsoned));
    }
}
