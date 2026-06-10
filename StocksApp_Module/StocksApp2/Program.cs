using Entities;
using ServiceContracts;
using Services;
using Servicess;
using StocksApp2;
using Microsoft.EntityFrameworkCore;
using RepositryContracts;
using Repositories;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

//builder.Host.ConfigureLogging(loggingprovider =>
//{
//    loggingprovider.AddConsole();
//    loggingprovider.AddDebug();
//    loggingprovider.AddEventLog();
//});


builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
       .ReadFrom.Services(services);

});

builder.Services.AddControllersWithViews();
// ========== REPOSITORY LAYER ==========
builder.Services.AddScoped<CountryRepositryContract, CountryRepository>();
builder.Services.AddScoped<PersonRepositryContract, PersonRepository>();

// ========== SERVICE LAYER ==========
builder.Services.AddScoped<ICountryServices, CountryServices>();
builder.Services.AddScoped<IPersonServices, PersonServices>();
builder.Services.AddScoped<ICountryServices, CountryServices>();
builder.Services.AddScoped<IPersonServices, PersonServices>();
builder.Services.AddScoped<PersonRepositryContract, PersonRepository>();
builder.Services.AddScoped<CountryRepositryContract, CountryRepository>();

// temp
/*
 Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ContectManagerDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False
 
 */

builder.Services.AddSingleton<IStocksService, StocksService>();

builder.Services.AddHttpClient<IFinnhubService, FinnhubService>(); 
//builder.Services.AddScoped<IStocksService, StocksService>();
builder.Services.AddDbContext<AppDBContext>(
    Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("ContactDb")  )
    );
builder.Services.AddDbContext<AppDBContext>();

builder.Services.AddControllersWithViews();
builder.Services.Configure<TradingOptions>(
    builder.Configuration.GetSection("TradingOptions"));

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});


var app = builder.Build();




//app.Logger.LogInformation("Log-info-message");
//app.Logger.LogWarning("Log-warning-message");
//app.Logger.LogError("Log-error-message");   
//app.Logger.LogCritical("Log-critical-message");
//app.Logger.LogDebug("Log-debug-message");


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

app.UseRouting();
app.MapControllers();

app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Trade}/{action=Index}/{id?}");

app.UseStaticFiles();


app.Run();

public partial class Program { }