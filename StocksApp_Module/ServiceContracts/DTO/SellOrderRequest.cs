using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class SellOrderRequest
    {


        [Required]
        public string StockSymbol { set; get; }

        [Required]
        public string StockName { set; get; }

        public DateTime DateAndTimeOfOrder { set; get; }
        [Range(1, 100000)]
        public uint Quantity { set; get; }//[Value should be between 1 and 100000]

        [Range(1, 100000)]
        public double Price { set; get; }//
        public async Task<SellOrderResponse> ConvertToSellOrderResponse()
        {

            var SellOrderID = Guid.NewGuid();

            return new SellOrderResponse
            {
                SellOrderID = SellOrderID,
                StockSymbol = this.StockSymbol,
                StockName = this.StockName,
                Price = this.Price,
                Quantity = this.Quantity,
                DateAndTimeOfOrder = this.DateAndTimeOfOrder,
                // Calculation: Total value of the trade
                TradeAmount = (double)this.Price * (double)this.Quantity
            };
        }
    }
}