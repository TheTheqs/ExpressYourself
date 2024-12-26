//All generics imports are in GlobalUsings
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache(); //Cache declaration needed.
builder.Services.AddHostedService<HS>();//Hosted Services, for the Update Database System.
builder.Services.AddHostedService<HT>();//Requisition test: Require a random ip every X minutes. Remove the comment to activate, access HT class to edit timer
builder.Services.AddHostedService<EH>();//Endpoint requisition automatic test, remove the comment to activate.
DB.Initialize(builder.Configuration.GetConnectionString("PostgresConnection")); //DB configuration for the raw sql usage (third task)
var app = builder.Build();
//Cache configuration
CM.Configure(app.Services.GetRequiredService<IMemoryCache>());
//JSON formating - This cannot stay in the DS class.
var options = new JsonSerializerOptions {WriteIndented = true};

app.MapGet("/", () => "Hello World! Hey");

//Get report endpoint - need to be before the get IP.
app.MapGet("/report", async (string? countries) => 
{
    try
    {
        if(string.IsNullOrEmpty(countries))
        {
            var result = DS.GetReports((await DB.GetAllReports(), "")); //Get the all reports from the data
            return Results.Ok(result);
        }
        else
        {
            var countryCodes = countries.Split(',') //convert the query parameters into a list
            .Select(code => code.Trim())
            .ToList();
            
            var result = DS.GetReports(await DB.GetSelectedReports(countryCodes)); //Make a call to get a filtered result.
            return Results.Ok(result);
        }
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
        return Results.Json(await DS.GetCountry(ipaddress), options); //Results
    } 
    catch(Exception err)
    {
        return Results.Problem(detail: err.Message, title: "[Error]An error occurred");
    }
});

app.Run();
