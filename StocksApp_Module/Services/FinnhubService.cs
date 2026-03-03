using ServiceContracts;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration; 

namespace Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FinnhubService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration; 
        }

        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(stockSymbol))
                throw new ArgumentNullException(nameof(stockSymbol));

            var token = _configuration["TradingOptions:FinnhubToken"];

            var response = await _httpClient.GetAsync(
                $"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}&token={token}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            var responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            return (responseDictionary == null || responseDictionary.Count == 0) ? null : responseDictionary;
        }


        public Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            throw new NotImplementedException();
        }
    }
}