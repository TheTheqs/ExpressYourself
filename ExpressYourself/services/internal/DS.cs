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

    public static async Task<object> GetUpdateReport(String[] strings)
    {
        try
        {
            if(string.IsNullOrEmpty(strings[0]))
            {
                strings[0] = "There is no IP to be Updated!";
            }
            if(string.IsNullOrEmpty(strings[1]))
            {
                strings[1] = "There is no IP to be Deleted!";
            }

            var jsoned = new
                {
                    updatedIPs = strings[0],
                    deletedIPs = strings[1] 
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
