//All generics imports are in GlobalUsings
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache(); //Cache declaration needed.
//builder.Services.AddHostedService<HS>();//Hosted Services, for the Update Database System.
DB.Initialize(builder.Configuration.GetConnectionString("PostgresConnection")); //DB configuration for the raw sql usage (third task)
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

//Get report endpoint - need to be before the get IP.
app.MapGet("/report", async (string? countries) => 
{
    try
    {
        if(string.IsNullOrEmpty(countries))
        {
            var result = DS.GetReports(await DB.GetAllReports());
            return Results.Ok(result);
        }
        return Results.NotFound(new {Message = "There is no Reports to be given"});
    }
    catch(Exception err)
    {
       return Results.Problem(detail: err.Message, title: "[Error]An error occurred");
    }
});

//Main endpoint. Used to get 
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
                //Try to get from the external IP
                ipaddress = await EA.GetIp(Ip);
                if(ipaddress == null)
                {
                    return Results.NotFound(new {Message = $"The requested IP {Ip} address could not be found or does not exist."}); //Return 404 when the IP does not exist in any source
                }
                //adding to the database
                if(await DB.SaveIP(ipaddress))
                {
                    Console.WriteLine($"[System]Ip {ipaddress.Ip} was saved in the DataBase");
                }
                else
                {
                    Console.WriteLine($"[Error]Failed to save new IP {ipaddress.Ip} in the DataBase");
                }
            }
            //adding to the cache
            if(await CM.Create<String, Ipaddress>(Ip, ipaddress))
            {
                Console.WriteLine($"[System]Ip {ipaddress.Ip} was saved in the Cache Memory");
            }
            else
            {
                Console.WriteLine($"[Error]Failed to IP add IP {ipaddress.Ip} in the cache");
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
        return Results.Problem(detail: err.Message, title: "[Error]An error occurred");
    }
});

app.Run();
