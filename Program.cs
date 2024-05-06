using ApiCoreTheHive.Data;
using ApiCoreTheHive.Helpers;
using ApiCoreTheHive.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors.Security;
using NSwag;
using ApiCoreTheHive.Helpers;
using Microsoft.Extensions.Azure;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
    (builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();

KeyVaultSecret secretSecretKey = await secretClient.GetSecretAsync("secretSecretKey");
KeyVaultSecret secretAudience = await secretClient.GetSecretAsync("secretAudience");
KeyVaultSecret secretIssuer = await secretClient.GetSecretAsync("secretIssuer");

string secretKey = secretSecretKey.Value;
string audience = secretAudience.Value;
string issuer = secretIssuer.Value;

KeyVaultSecret secret = await secretClient.GetSecretAsync("secretConnectionString"); 
string connectionString = secret.Value;


HelperActionServicesOAuth helper = new HelperActionServicesOAuth(secretKey, audience, issuer);

builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);


builder.Services.AddAuthentication
    (helper.GetAuthenticateSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

// ANTIGUO
// string connectionString = builder.Configuration.GetConnectionString("SqlAzure");

builder.Services.AddTransient<RepositoryTheHive>();
builder.Services.AddDbContext<TheHiveContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// REGISTRAMOS SWAGGER COMO SERVICIO

builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "API TheHive";
    document.Description = "Una api normal y corriente";
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();
app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.InjectStylesheet("/css/theme-material.css");
    options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "TheHive API");
    options.RoutePrefix = "";
});


if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();