using ExpressYourself.Infrastructure.DependencyInjection;
using Microsoft.OpenApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region Services

// Controllers
builder.Services.AddControllers();

// OpenAPI / Swagger
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ExpressYourself API",
        Version = "v1",
        Description = "Technical assignment API for IP information and reports."
    });

    // XML documentation integration
    // This enables Swagger to read XML comments from controllers and models.
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

    if (File.Exists(xmlFilePath))
    {
        options.IncludeXmlComments(xmlFilePath);
    }
});

// Dependency Injection
builder.Services.AddInfrastructure(builder.Configuration);

// Register future application services here.
// Example:
// builder.Services.AddApplication();

#endregion

var app = builder.Build();

#region Middleware Pipeline

// Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ExpressYourself API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();

public partial class Program
{
}