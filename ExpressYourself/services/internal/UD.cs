using System.Text;
namespace ExpressYourself.services;
//This class holds the Update System logic, but not the automation aspect.
public static class UD // Stands for Update Data!!
{
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
            List<String> changed = new List<String>(); //To reference the changed IPs
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
                        changed.Add(ip.Ip);
                    }
                }
            //Finishing fuction
            String[] response = {BuildString(changed), BuildString(deleted)};
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
