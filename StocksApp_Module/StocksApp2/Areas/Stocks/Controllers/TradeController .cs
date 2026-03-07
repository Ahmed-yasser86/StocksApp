using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;
using System.Text.Json;

namespace StocksApp2.Areas.StocksUIComponent.Controllers
{

    [Area("stcoks")]
    public class TradeController : Controller
    {
        private readonly IOptions<TradingOptions> _tradingOptions;
        private readonly IFinnhubService _finnhubService;
        private readonly IStocksService _stocksService;
        public TradeController(IFinnhubService finnhubService , IOptions<TradingOptions> tradingop)
        {

            _finnhubService = finnhubService;

            _tradingOptions = tradingop;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var symbol = _tradingOptions.Value.DefaultStockSymbol;
            var token = _tradingOptions.Value.FinnhubToken; // get token from appsettings

            var profile = await _finnhubService.GetCompanyProfile(symbol);
            var priceQuote = await _finnhubService.GetStockPriceQuote(symbol);
            var model = new StockTrade
            {
                StockSymbol = symbol,
                StockName = ((JsonElement)profile["name"]).GetString(),
                Price = ((JsonElement)priceQuote["c"]).GetDouble(),
                Quantity = 1
            };

            ViewBag.FinnhubToken = token; // pass token to view
            return View("Index",model);
        }
    
    }
}
