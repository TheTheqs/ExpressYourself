using System.Text;
namespace ExpressYourself.services;
//This class holds the Update System logic, but not the automation aspect.
public static class UD // Stands for Update Data!!
{
    //The following function will get every IP in the Database (100 by 100) and call the update funtion for each pack
    public static async Task<String> UpdateDataBase()
    {
        Console.WriteLine("Starting database update...");
        StringBuilder updated = new StringBuilder().Append("Updated IPs: ");
        StringBuilder deleted = new StringBuilder().Append("Deleted Ips: ");
        try
        {

            int totalPacks = (int)Math.Ceiling((Double)await DB.GetIpCount()/100); // get the next integer number from the total ips /100.
            if(totalPacks <= 0)
            {
                return "Error at total packs calculation";
            }
            for(int i = 0; i < totalPacks; i ++)
            {
                int startIndex = i*100;
                int endIndex = startIndex + 100;
                List<Ipaddress> ipaddresses = await DB.GetIpaddressesPack(startIndex, endIndex);
                if(ipaddresses.Count == 0)
                {
                    return "Error at getting packed IPs: list is empty";
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
            Console.WriteLine("IP list is empty or null.");
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
                    Console.WriteLine("Vindo aqui");
                    Ipaddress? reference = await EA.GetIp(ip.Ip); //Getting the reference from EA.
                    if(reference == null) //This ip is no longer accessible
                    {
                        if(await DB.DeleteIP(ip) == false)
                        {
                            Console.WriteLine("Failed at IP destruction");
                        }
                        deleted.Add(ip.Ip);
                        if(await CM.HasKey(ip.Ip))
                        {
                            await CM.Delete(ip.Ip);
                        }
                    } 
                    else if (reference.CountryId != ip.CountryId)
                    {
                        if(await DB.UpdateIP(reference) == false)
                        {
                            Console.WriteLine("Failed at IP Update");
                        }
                        if(await CM.HasKey(ip.Ip))
                        {
                            await CM.Update(ip.Ip, reference);
                        }
                        updated.Add(ip.Ip);
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
