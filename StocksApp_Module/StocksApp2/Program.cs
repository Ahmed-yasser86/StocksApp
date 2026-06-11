using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using RepositryContracts;
using Serilog;
using ServiceContracts;
using Services;
using Servicess;
using RepositoryContracts; // For IStocksRepository
using Repositories_Stocks; // For StocksRepository
using StocksApp2;
using ServiceContractsContacts;
using EntitiesStocks;
using Entities;
var builder = WebApplication.CreateBuilder(args);

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

// ========== FINNHUB (NO DATABASE) ==========
// Register HttpClient for FinnhubRepository
builder.Services.AddHttpClient<IFinnhubRepository, FinnhubRepository>();

// Register Repository and Service
builder.Services.AddScoped<IFinnhubRepository, FinnhubRepository>();
builder.Services.AddScoped<IFinnhubService, FinnhubService>();

// ========== STOCKS (WITH DATABASE) ==========
// Register Stocks Repository
builder.Services.AddScoped<IStocksRepository, StocksRepository>();  // ← YOU MISSED THIS!

// Register Stocks Service (USE SCOPED, NOT SINGLETON!)
builder.Services.AddScoped<IStocksService, StocksService>();  // Changed from Singleton to Scoped

// ========== DATABASE CONTEXTS ==========
builder.Services.AddDbContext<AppDBContext>(
    Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("ContactDb"))
);

// For Stocks DbContext

builder.Services.AddDbContext<StocksDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("StocksDbConnection"),
        b => b.MigrationsAssembly("EntitiesStocks")
    ));

// ========== CONFIGURATION ==========
builder.Services.Configure<TradingOptions>(
    builder.Configuration.GetSection("TradingOptions"));

// ========== HTTP LOGGING ==========
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();

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