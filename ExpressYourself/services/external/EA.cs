using System.Net.Http;
using System.Threading.Tasks;

namespace ExpressYourself.services;
//This classe deals with the External API IP2C. All interactions with IP2C happens here.
public static class EA //Stands for External API
{
    private static readonly HttpClient httpClient = new HttpClient(); //static relation attribute

    public static async Task<Ipaddress?> GetIp(String ip)
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync("https://ip2c.org/" + ip);
            response.EnsureSuccessStatusCode(); //Requisition confirmation
            string responseBody = await response.Content.ReadAsStringAsync(); //Turn response to string (csv originally)
            string[] lines = responseBody.Split('\n', StringSplitOptions.RemoveEmptyEntries); //Split the response to get the attributes
            if (lines.Length == 0)
            {
                return null; //Return null for an empty response
            }
            string[] columns = lines[0].Split(';');
            if(columns[1] == null)
            {
                return null; //Return null for an unexpected error (format error)
            }
            if(columns[3] is string columnValue)
            {
                columns[3] = columnValue.Substring(0, Math.Min(columnValue.Length, 50)); //This will assure that the new Country name can be added to the DataBase
            }
            Country? country = new Country(columns[3], columns[1].Trim(), columns[2].Trim());
            if(await DB.HasCountry(columns[1]))
            {
                country = await DB.GetCountryByCode(columns[1]);
            }
            else
            {
                Console.WriteLine($"[Update]New Country was found: {columns[3]}. Addition to the DataBase will be requested!");
                country = await DB.SaveCountry(country);
            }
            if(country != null)
            {
                Console.WriteLine($"[Request] The IP {ip} was successefully requested from the External API");
                return new Ipaddress(ip, country); //Create a new Ip obj. The NewIpAsync assure that the country will be added on data if not existed.
            }
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        Console.WriteLine($"[Request] The IP {ip} has no reference in the the External API");
        return null;
    }
}