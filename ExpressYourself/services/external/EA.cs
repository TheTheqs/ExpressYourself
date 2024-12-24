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
            Country? country = new Country(columns[3], columns[1].Trim(), columns[2].Trim());
            if(await DB.HasCountry(columns[1]))
            {
                Console.WriteLine("Country is already in the DataBase");
                country = await DB.GetCountryByCode(columns[1]);
            }
            else
            {
                country = await DB.SaveCountry(country);
            }
            Console.WriteLine("Getting from the EA");
            if(country != null)
            {
                return new Ipaddress(ip, country); //Create a new Ip obj. The NewIpAsync assure that the country will be added on data if not existed.
            }
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return null;
    }
}