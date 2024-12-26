using System.Data;
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

    //The follow function create a log response for the Update Internal process! Not to be confused with the report system to the client!!!!
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
    //The following function takes a DataTable and converts it into a serializable list of dictionaries.
    public static List<Dictionary<String, object>>? GetReports(DataTable? dataTable)
    {
        if(dataTable == null || dataTable.Rows.Count == 0)
        {
            return null; //to avoid null or empty data.
        }
        try{
            var list = new List<Dictionary<String, object>>(); //the returned list.
            
            foreach (DataRow row in dataTable.Rows) //Data table iteration
            {
                var dictionary = new Dictionary<String, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    dictionary[column.ColumnName] = row[column] ?? DBNull.Value; //build every obj attribute
                }
                list.Add(dictionary);             
            }
            return list; //return a populed list
        }
        catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
        return null; //return null if something goes worong.
    }
}
