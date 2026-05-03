using ServiceContracts;
using Services;
using Servicess;
using StocksApp2;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ICountryServices, CountryServices>();
builder.Services.AddSingleton<IPersonServices, PersonServices>();

// temp
builder.Services.AddSingleton<IStocksService, StocksService>();

builder.Services.AddHttpClient<IFinnhubService, FinnhubService>(); 
//builder.Services.AddScoped<IStocksService, StocksService>();

builder.Services.AddControllersWithViews();
builder.Services.Configure<TradingOptions>(
    builder.Configuration.GetSection("TradingOptions"));
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}



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

