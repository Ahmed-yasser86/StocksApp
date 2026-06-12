using AutoFixture;
using Castle.Core.Logging;
using Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StocksUnitTest
{
    public class StocksUnitTest
    {
        private readonly IStocksService _stocksService;
        private readonly Mock<IStocksRepository> _stocksRepositoryMock;
        private readonly IFixture _fixture;

        public StocksUnitTest()
        {
            _fixture = new Fixture();
            _stocksRepositoryMock = new Mock<IStocksRepository>();

            var loggerMock = new Mock<ILogger<StocksService>>();
            ILogger<StocksService> logger = loggerMock.Object;

            _stocksService = new StocksService(_stocksRepositoryMock.Object, logger);
        }

        #region CreateBuyOrder Tests

        [Fact]
        public async Task CreateBuyOrder_NullRequest_ShouldThrowArgumentNullException()
        {
            // Arrange
            BuyOrderRequest? request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _stocksService.CreateBuyOrder(request));
        }

        [Fact]
        public async Task CreateBuyOrder_NullStockName_ShouldThrowArgumentException()
        {
            // Arrange
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(p => p.StockName, (string?)null)
                .With(p => p.StockSymbol, "MSFT")
                .With(p => p.Price, 100)
                .With(p => p.Quantity, (uint)10)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _stocksService.CreateBuyOrder(request));
        }

        [Fact]
        public async Task CreateBuyOrder_NullStockSymbol_ShouldThrowArgumentException()
        {
            // Arrange
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(p => p.StockSymbol, (string?)null)
                .With(p => p.StockName, "Microsoft")
                .With(p => p.Price, 100)
                .With(p => p.Quantity, (uint)10)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _stocksService.CreateBuyOrder(request));
        }

        [Fact]
        public async Task CreateBuyOrder_QuantityLessThanOne_ShouldThrowArgumentException()
        {
            // Arrange
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(p => p.StockSymbol, "MSFT")
                .With(p => p.StockName, "Microsoft")
                .With(p => p.Price, 100)
                .With(p => p.Quantity, (uint)0)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _stocksService.CreateBuyOrder(request));
        }

        [Fact]
        public async Task CreateBuyOrder_PriceIsZero_ShouldThrowArgumentException()
        {
            // Arrange
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(p => p.StockSymbol, "MSFT")
                .With(p => p.StockName, "Microsoft")
                .With(p => p.Price, 0)
                .With(p => p.Quantity, (uint)10)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _stocksService.CreateBuyOrder(request));
        }

        [Fact]
        public async Task CreateBuyOrder_ValidData_ShouldReturnSuccessResponse()
        {
            // Arrange
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(p => p.StockSymbol, "MSFT")
                .With(p => p.StockName, "Microsoft")
                .With(p => p.Price, 150.50)
                .With(p => p.Quantity, (uint)10)
                .Create();

            BuyOrder buyOrder = request.ToBuyOrder();
            buyOrder.BuyOrderID = Guid.NewGuid();

            _stocksRepositoryMock.Setup(repo => repo.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);

            // Act
            BuyOrderResponse response = await _stocksService.CreateBuyOrder(request);

            // Assert
            response.Should().NotBeNull();
            response.BuyOrderID.Should().NotBe(Guid.Empty);
            response.StockSymbol.Should().Be(request.StockSymbol);
            response.StockName.Should().Be(request.StockName);
            response.Price.Should().Be(request.Price);
            response.Quantity.Should().Be(request.Quantity);
            response.TradeAmount.Should().Be(request.Price * request.Quantity);
        }

        #endregion

        #region CreateSellOrder Tests

        [Fact]
        public async Task CreateSellOrder_NullRequest_ShouldThrowArgumentNullException()
        {
            // Arrange
            SellOrderRequest? request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _stocksService.CreateSellOrder(request));
        }

        [Fact]
        public async Task CreateSellOrder_PriceIsZero_ShouldThrowArgumentException()
        {
            // Arrange
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(p => p.StockSymbol, "MSFT")
                .With(p => p.StockName, "Microsoft")
                .With(p => p.Price, 0)
                .With(p => p.Quantity, (uint)10)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _stocksService.CreateSellOrder(request));
        }

        [Fact]
        public async Task CreateSellOrder_ValidData_ShouldReturnSuccessResponse()
        {
            // Arrange
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(p => p.StockSymbol, "AAPL")
                .With(p => p.StockName, "Apple")
                .With(p => p.Price, 200.00)
                .With(p => p.Quantity, (uint)5)
                .Create();

            SellOrder sellOrder = request.ToSellOrder();
            sellOrder.SellOrderID = Guid.NewGuid();

            _stocksRepositoryMock.Setup(repo => repo.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);

            // Act
            SellOrderResponse response = await _stocksService.CreateSellOrder(request);

            // Assert
            response.Should().NotBeNull();
            response.SellOrderID.Should().NotBe(Guid.Empty);
            response.StockSymbol.Should().Be(request.StockSymbol);
            response.StockName.Should().Be(request.StockName);
            response.Price.Should().Be(request.Price);
            response.Quantity.Should().Be(request.Quantity);
            response.TradeAmount.Should().Be(request.Price * request.Quantity);
        }

        #endregion

        #region GetBuyOrders Tests

        [Fact]
        public async Task GetBuyOrders_DefaultList_ShouldBeEmpty()
        {
            // Arrange
            _stocksRepositoryMock.Setup(repo => repo.GetBuyOrders())
                .ReturnsAsync(new List<BuyOrder>());

            // Act
            List<BuyOrderResponse> result = await _stocksService.GetBuyOrders();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
       
        public async Task GetBuyOrders_AddFewOrders_ShouldReturnCorrectList()
        {
            // Arrange
            List<BuyOrder> buyOrders = new List<BuyOrder>
    {
        _fixture.Build<BuyOrder>()
            .With(p => p.StockSymbol, "MSFT")
            .With(p => p.StockName, "Microsoft")
            .With(p => p.Price, 100)
            .With(p => p.Quantity, (uint)1)
            .With(p => p.DateAndTimeOfOrder, DateTime.Parse("2023-01-01"))
            .Create(),

        _fixture.Build<BuyOrder>()
            .With(p => p.StockSymbol, "AAPL")
            .With(p => p.StockName, "Apple")
            .With(p => p.Price, 200)
            .With(p => p.Quantity, (uint)2)
            .With(p => p.DateAndTimeOfOrder, DateTime.Parse("2023-01-02"))
            .Create()
    };

            // مهم: رتب البيانات هنا قبل ما ترجعها في الـ Mock!
            var sortedBuyOrders = buyOrders.OrderByDescending(b => b.DateAndTimeOfOrder).ToList();

            _stocksRepositoryMock.Setup(repo => repo.GetBuyOrders())
                .ReturnsAsync(sortedBuyOrders);  // ← استخدم المصفوفة المرتبة

            // Act
            List<BuyOrderResponse> result = await _stocksService.GetBuyOrders();

            // Assert
            result.Should().HaveCount(2);
            result[0].StockSymbol.Should().Be("AAPL"); // الآن هتكون الأولى ✅
            result[1].StockSymbol.Should().Be("MSFT");
        }

        #endregion

        #region GetSellOrders Tests

        [Fact]
        public async Task GetSellOrders_DefaultList_ShouldBeEmpty()
        {
            // Arrange
            _stocksRepositoryMock.Setup(repo => repo.GetSellOrders())
                .ReturnsAsync(new List<SellOrder>());

            // Act
            List<SellOrderResponse> result = await _stocksService.GetSellOrders();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSellOrders_AddFewOrders_ShouldReturnCorrectList()
        {
            // Arrange
            List<SellOrder> sellOrders = new List<SellOrder>
            {
                _fixture.Build<SellOrder>()
                    .With(p => p.StockSymbol, "TSLA")
                    .With(p => p.StockName, "Tesla")
                    .With(p => p.Price, 500)
                    .With(p => p.Quantity, (uint)10)
                    .With(p => p.DateAndTimeOfOrder, DateTime.Parse("2023-01-02"))
                    .Create(),

                _fixture.Build<SellOrder>()
                    .With(p => p.StockSymbol, "GOOGL")
                    .With(p => p.StockName, "Google")
                    .With(p => p.Price, 150)
                    .With(p => p.Quantity, (uint)5)
                    .With(p => p.DateAndTimeOfOrder, DateTime.Parse("2023-01-01"))
                    .Create()
            };

            _stocksRepositoryMock.Setup(repo => repo.GetSellOrders())
                .ReturnsAsync(sellOrders);

            // Act
            List<SellOrderResponse> result = await _stocksService.GetSellOrders();

            // Assert
            result.Should().HaveCount(2);
            result[0].StockSymbol.Should().Be("TSLA"); // Newest first
        }

        #endregion

        #region Integration Style Tests (With Repository)

        [Fact]
        public async Task CreateBuyOrder_ThenGetBuyOrders_ShouldIncludeNewOrder()
        {
            // Arrange
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(p => p.StockSymbol, "MSFT")
                .With(p => p.StockName, "Microsoft")
                .With(p => p.Price, 150.50)
                .With(p => p.Quantity, (uint)10)
                .Create();

            BuyOrder buyOrder = request.ToBuyOrder();
            buyOrder.BuyOrderID = Guid.NewGuid();
            buyOrder.DateAndTimeOfOrder = DateTime.Now;

            List<BuyOrder> buyOrdersList = new List<BuyOrder>();

            _stocksRepositoryMock.Setup(repo => repo.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .Callback<BuyOrder>(order => buyOrdersList.Add(order))
                .ReturnsAsync((BuyOrder order) => order);

            _stocksRepositoryMock.Setup(repo => repo.GetBuyOrders())
                .ReturnsAsync(() => buyOrdersList.OrderByDescending(o => o.DateAndTimeOfOrder).ToList());

            // Act
            BuyOrderResponse created = await _stocksService.CreateBuyOrder(request);
            List<BuyOrderResponse> allOrders = await _stocksService.GetBuyOrders();

            // Assert
            allOrders.Should().ContainSingle();
            allOrders[0].BuyOrderID.Should().Be(created.BuyOrderID);
        }

        #endregion
    }
}