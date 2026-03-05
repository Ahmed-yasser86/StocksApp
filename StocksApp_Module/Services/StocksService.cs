using Microsoft.Extensions.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using Servicess.Helpers;
using StocksApp2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StocksService : IStocksService
    {
        private readonly IOptions<TradingOptions> _tradingOptions;

        private readonly IFinnhubService _finnhubService;
        List<BuyOrderResponse> buyOrderResponsesList;
        
        public   StocksService(IFinnhubService finnhubService ) { 
        
        _finnhubService = finnhubService;

         buyOrderResponsesList = new List<BuyOrderResponse>();


        }

        // impppportantttttttttttttttttttttttttttttttttttttttttt!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // don't forget adding cancellition token when u add EF Core layer 
        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
        {
            if(buyOrderRequest == null)
            {
                throw new ArgumentNullException(nameof(buyOrderRequest));
            }

            ValidationHelpers.ValidationFunction(buyOrderRequest);


            if (buyOrderRequest.Price < 0)
            {
                throw new Exception("Can Not insert Price value less than or equal 0");
            }
            if (buyOrderRequest.Quantity < 0)
            {
                throw new Exception("Can Not insert Quantity less than 1");
            }

            var respons = await buyOrderRequest.ConvertToBuyOrederRespones();

            buyOrderResponsesList.AddRange(respons);

            return respons;
        }

        List<SellOrderResponse> sellOrderResponsesList = new List<SellOrderResponse>();

        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
        {
            if (sellOrderRequest == null)
            {
                throw new ArgumentNullException(nameof(sellOrderRequest));
            }

            ValidationHelpers.ValidationFunction(sellOrderRequest);

            var response = await sellOrderRequest.ConvertToSellOrderResponse();

            sellOrderResponsesList.Add(response);

            return response;
        }
        public Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            // Return the list ordered by date (newest first)
            return Task.FromResult(buyOrderResponsesList
                .OrderByDescending(temp => temp.DateAndTimeOfOrder)
                .ToList());
        }

        public Task<List<SellOrderResponse>> GetSellOrders()
        {
            return Task.FromResult(sellOrderResponsesList
                .OrderByDescending(temp => temp.DateAndTimeOfOrder)
                .ToList());
        }
    }
}
