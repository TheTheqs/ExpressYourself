//All generics imports are in GlobalUsings
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache(); //Cache declaration needed.
var app = builder.Build();
//Cache configuration
CM.Configure(app.Services.GetRequiredService<IMemoryCache>());
//Ip in response control variable
bool sendIp = false;
//JSON formating - This cannot stay in the DS class.
var options = new JsonSerializerOptions {WriteIndented = true};

app.MapGet("/", () => "Hello World!");

//Util endpoint for sendip control
app.MapPost("/toggleip", () =>
{
    sendIp = !sendIp;
    return Results.Ok(sendIp);
});

app.MapGet("/{Ip}", async (String Ip) => 
{
    try
    {   
        //Try to get from the Cache
        Ipaddress? ipaddress = await CM.Get<String>(Ip);
        if(ipaddress == null)
        {
            //Try to get from the DataBase
            ipaddress = await DB.GetIpaddress(Ip);
            if(ipaddress == null)
            {
                return Results.NotFound(); //Return 404 when the IP does not exist in the date (just a test)
            }
            //adding to the cache
            if(await CM.Create<String, Ipaddress>(Ip, ipaddress))
            {
                Console.WriteLine("Added to the Cache");
            }
            else
            {
                Console.WriteLine("Failed to add on the cache");
            }
        }
        //Requisition finals
        if(sendIp)
        {
            return Results.Json(await DS.GetIpCountry(ipaddress), options); //Results with IP in it.
        }
        return Results.Json(await DS.GetCountry(ipaddress), options); //Results without IP in it.
    } 
    catch(Exception err)
    {
        return Results.Problem(detail: err.Message, title: "An error occurred");
    }
});

app.Run();
