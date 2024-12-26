using System.Text;
namespace ExpressYourself.services;
//This class holds the Update System logic, but not the automation aspect.
public static class UD // Stands for Update Data!!
{
    //The following function will get every IP in the Database (100 by 100) and call the update funtion for each pack
    public static async Task<String> UpdateDataBase()
    {
        Console.WriteLine("[Update]Starting database update...");
        StringBuilder updated = new StringBuilder().Append("[Update]Updated IPs: ");
        StringBuilder deleted = new StringBuilder().Append("Deleted Ips: ");
        try
        {
            int totalIp = await DB.GetIpCount();
            Console.WriteLine($"[Update]Total Ips in the Database: {totalIp}");
            int totalPacks = (int)Math.Ceiling((Double)totalIp/100); // get the next integer number from the total ips /100.
            if(totalPacks <= 0)
            {
                return "Error at total packs calculation";
            }
            Console.WriteLine($"[Update]Cicles needed: {totalPacks}");
            for(int i = 0; i < totalPacks; i ++)
            {
                Console.WriteLine($"[Update]Staring Cicle nº {i + 1}");
                int startIndex = i*100;
                int endIndex = startIndex + 100;
                List<Ipaddress> ipaddresses = await DB.GetIpaddressesPack(startIndex, endIndex);
                if(ipaddresses.Count == 0)
                {
                    Console.WriteLine("Error at getting packed IPs: list is empty");
                    continue;
                }
                String[] response = await UpdateAdresses(ipaddresses);
                updated.Append(response[0]);
                deleted.Append(response[1]);
            }
            String report = updated.ToString() + "\n" + deleted.ToString(); //Building response.
            return report;
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return "Error at update call!";
    }

    //Update Function
    public static async Task<String[]> UpdateAdresses(List<Ipaddress> ipList)
    {   
        if (ipList == null || !ipList.Any()) //List validation
        {
            Console.WriteLine("[Error]IP list is empty or null.");
            return new string[] { "", "" };
        }
        try
        {
            //Result Strings
            List<String> updated = new List<String>(); //To reference the changed IPs
            List<String> deleted = new List<String>(); //To reference the IP that are no longer acessible.
            //Comparison
            foreach (Ipaddress ip in ipList)
                {
                    Ipaddress? reference = await EA.GetIp(ip.Ip); //Getting the reference from EA.
                    if(reference == null) //This ip is no longer accessible
                    {
                        if(await DB.DeleteIP(ip) == false)
                        {
                            Console.WriteLine("[Error]Failed at IP destruction");
                            continue;
                        }
                        deleted.Add(ip.Ip);
                        if(await CM.HasKey(ip.Ip))
                        {
                            await CM.Delete(ip.Ip);

                        }
                        Console.WriteLine($"[Update]The Ip {ip} no longer has a reference and has been deleted.");
                    } 
                    else if (reference.CountryId != ip.CountryId)
                    {
                        Console.WriteLine($"[Update]Saved Ip and Reference has diferent Origin. Requiring Update...");
                        if(await DB.UpdateIP(reference) == false)
                        {
                            Console.WriteLine("[Error]Failed at IP Update");
                        }
                        if(await CM.HasKey(ip.Ip))
                        {
                            await CM.Update(ip.Ip, reference);
                        }
                        updated.Add(ip.Ip);
                    }
                    else
                    {
                        Console.WriteLine($"[Update]The Ip {ip.Ip} and its reference has the same origin.");
                    }
                }
            //Finishing fuction
            String[] response = {BuildString(updated), BuildString(deleted)};
            return response;
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        String[] nullResponse = {"", ""};
        return nullResponse;
    }

    //Build string function for log and report
    public static String BuildString(List<String> stringList)
    {
        StringBuilder stringed = new StringBuilder();
        foreach(String stri in stringList)
        {
            stringed.Append(stri + "; ");
        }
        return stringed.ToString();
    }
}
