using System.Net.Http;
using System.Threading.Tasks;

namespace ExpressYourself.services;
//This class makes a random request for the api
public static class RT //Stands for REQUEST TEST
{
    private static Random random = new Random();
    private static String url = "http://localhost:5289/";
    private static readonly HttpClient client = new HttpClient();
    private static string[] ips = {
    "138.181.172.200",
    "64.78.29.251",
    "225.35.164.222",
    "144.14.34.212",
    "29.82.217.36",
    "89.247.79.11",
    "150.76.146.186",
    "63.63.114.237",
    "126.39.140.106",
    "139.93.76.1",
    "114.3.156.202",
    "52.229.127.211",
    "205.126.83.9",
    "160.132.65.235",
    "75.198.193.130",
    "181.18.87.87",
    "97.10.235.187",
    "180.214.132.175",
    "71.219.68.40",
    "191.55.29.159",
    "137.197.50.98",
    "83.202.244.60",
    "3.240.26.255",
    "113.189.108.201",
    "77.4.45.139",
    "180.215.203.76",
    "179.21.137.10",
    "46.254.56.52",
    "46.116.127.42",
    "48.77.107.146",
    "94.151.163.112",
    "234.137.154.203",
    "110.193.4.56",
    "215.118.41.19",
    "121.126.91.58",
    "43.245.6.6",
    "227.108.172.174",
    "142.84.248.167",
    "29.198.138.176",
    "235.45.183.134",
    "43.237.252.108",
    "93.40.193.50",
    "198.214.117.48",
    "207.197.131.37",
    "37.79.154.6",
    "227.21.248.110",
    "52.202.73.188",
    "111.46.63.93",
    "1.164.60.246",
    "121.47.66.206",
    "170.8.71.158",
    "225.0.85.11",
    "92.238.177.143",
    "19.127.105.11",
    "238.89.63.2",
    "218.215.99.220",
    "225.164.212.41",
    "12.102.149.49",
    "159.10.229.67",
    "100.7.108.45",
    "167.81.206.142",
    "163.26.242.172",
    "150.240.51.201",
    "174.236.145.245",
    "205.198.209.22",
    "233.24.15.249",
    "39.106.118.239",
    "85.200.162.191",
    "131.148.122.166",
    "188.210.200.72",
    "162.81.97.106",
    "201.239.30.170",
    "63.87.249.228",
    "196.229.93.221",
    "186.63.46.27",
    "220.214.76.220",
    "109.246.45.19",
    "30.128.60.242",
    "232.172.28.250",
    "228.98.99.254",
    "148.172.210.132",
    "115.195.149.100",
    "68.69.126.255",
    "105.19.202.116",
    "232.173.244.224",
    "8.190.247.151",
    "106.216.144.179",
    "123.158.85.21",
    "65.133.56.43",
    "220.183.255.161",
    "203.57.46.221",
    "5.200.237.197",
    "180.213.156.55",
    "84.32.132.21",
    "92.187.10.22",
    "113.213.237.240",
    "41.125.207.28",
    "227.160.59.212",
    "213.152.90.37",
    "69.55.53.52"
    }; //Possible IPs
   //Return a random IP from the array and to request its origin from the API
   public static async Task<String?> ReqRandomIP()
   {
        Console.WriteLine("[RequestTest]Getting random IP..");
            String nIp = GetRandomIP();
            Console.WriteLine($"[RequestTest]Selected IP: {nIp}");
            try
            {
                Console.WriteLine("[RequestTest]Trying Requisition..");
                HttpResponseMessage response = await client.GetAsync(url + nIp); //trying requisition
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

   private static String GetRandomIP()
   {
        return ips[random.Next(ips.Length)];
   }
}