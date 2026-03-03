using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using ServiceContracts;
using Services;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StocksUnitTest
{
    public class FinnhubServiceTest1
    {
        private readonly IFinnhubService _finnhubService;
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly IConfiguration _configuration;

        public FinnhubServiceTest1()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_handlerMock.Object);

            var myConfiguration = new Dictionary<string, string>
            {
                {"TradingOptions:FinnhubToken", "ABC_TEST_TOKEN"},
                {"TradingOptions:DefaultStockSymbol", "MSFT"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _finnhubService = new FinnhubService(httpClient, _configuration);
        }

        //  PRIVATE HELPER (The "Cleaner" Method) 
        private void MockHttpResponse(HttpStatusCode statusCode, string content)
        {
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                });
        }

        // --- UNIT TESTS ---

        [Fact]
        public async Task GetCompanyProfile_NullOrEmpty_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _finnhubService.GetCompanyProfile(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _finnhubService.GetCompanyProfile(""));
        }

        [Fact]
        public async Task GetCompanyProfile_ProperStockSymbol_ShouldReturnData()
        {
            // Arrange
            string mockJson = "{\"name\":\"Microsoft Corp\",\"ticker\":\"MSFT\"}";
            MockHttpResponse(HttpStatusCode.OK, mockJson);

            // Act
            var result = await _finnhubService.GetCompanyProfile("MSFT");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Microsoft Corp", result!["name"].ToString());
            Assert.Equal("MSFT", result["ticker"].ToString());
        }

        [Fact]
        public async Task GetCompanyProfile_InvalidStockSymbol_ShouldReturnNull()
        {
            MockHttpResponse(HttpStatusCode.OK, "{}");

            // Act
            var result = await _finnhubService.GetCompanyProfile("FAKE_SYMBOL");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCompanyProfile_WhenApiKeyIsInvalid_ShouldThrowException()
        {
            // Arrange
            MockHttpResponse(HttpStatusCode.Unauthorized, "Invalid API Key");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _finnhubService.GetCompanyProfile("AAPL"));
        }

        [Fact]
        public async Task GetCompanyProfile_WhenSomeFieldsAreMissing_ShouldStillReturnData()
        {
            string partialJson = "{\"name\":\"Apple Inc\",\"ticker\":\"AAPL\"}";
            MockHttpResponse(HttpStatusCode.OK, partialJson);

            // Act
            var result = await _finnhubService.GetCompanyProfile("AAPL");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Apple Inc", result!["name"].ToString());
            Assert.False(result.ContainsKey("phone")); 
        }

        [Fact]
        //server error or network error 
        public async Task GetCompanyProfile_WhenServerIsDown_ShouldThrowException()
        {
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("No network connection"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _finnhubService.GetCompanyProfile("AAPL"));
        }
    }
}