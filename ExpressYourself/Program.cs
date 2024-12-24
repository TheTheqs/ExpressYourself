//All generics imports are in GlobalUsings
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Ip in response control variable
bool sendIp = false;

app.MapGet("/", () => "Hello World!");

app.MapGet("/{Ip}", async (string Ip) => 
{
    try
    {
        Ipaddress? ipaddress = await DB.GetIpaddress(Ip); // "?" needed to remove the warning
        if(ipaddress == null)
        {
            return Results.NotFound(); //Return 404 when the IP does not exist in the date (just a test)
        }
            if(sendIp)
        {
            return Results.Json(await DS.GetIpCountry(ipaddress)); //Results with IP in it.
        }
        return Results.Json(await DS.GetCountry(ipaddress)); //Results without IP in it.
    } 
    catch(Exception err)
    {
        return Results.Problem(detail: err.Message, title: "An error occurred");
    }
});

app.Run();
