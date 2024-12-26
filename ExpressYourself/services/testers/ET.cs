using System.Net.Http;
using System.Threading.Tasks;

namespace ExpressYourself.services;
//This class makes a random request for the api based in a random endpoint
public static class ET //Stands for ENDPOINT TEST
{
    private static Random random = new Random();
    private static String url = "http://localhost:5289/";
    private static readonly HttpClient client = new HttpClient();
    private static string[] ips = {"definitelyNotAnValidIP", "report", "50.255.255.254", "report?countries=US,BR,DE,IE", "155.207.0.0", "report?countries=Marina,RU,CN,JP,DefinitelyNotACountry"}; //Possible EndPoints
   //Return a random ENDPOINT.
   public static async Task<String?> ReqRandomIP()
   {
        Console.WriteLine("[EndPointTest]Getting random IP..");
            String nEp = GetRandomEndPoint();
            Console.WriteLine($"[RequestTest]Selected IP: {nEp}");
            try
            {
                Console.WriteLine("[RequestTest]Trying Requisition..");
                HttpResponseMessage response = await client.GetAsync(url + nEp); //trying requisition
                string responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {responseData}");
                Console.WriteLine("[RequestTest]Requisistion Successed!");
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
                Console.WriteLine("[RequestTest]Requisistion Failed");
            }
        return "Failed";
   }

   private static String GetRandomEndPoint()
   {
        return ips[random.Next(ips.Length)];
   }
}