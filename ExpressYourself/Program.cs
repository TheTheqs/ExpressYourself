//All generics imports are in GlobalUsings
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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
