using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class BuyOrderRequest
    {
        [Required]
       public string StockSymbol { set; get; }

        [Required]
      public  string StockName { set; get; }

   public     DateTime DateAndTimeOfOrder { set; get; }
        [Range(1, 100000)]
     public   uint Quantity { set; get; }//[Value should be between 1 and 100000]

        [Range(1, 100000)]
      public  double Price { set; get; }//



        public async Task<BuyOrderResponse> ConvertToBuyOrederRespones()
        {


            return new BuyOrderResponse { BuyOrderID = Guid.NewGuid(), 
                DateAndTimeOfOrder= DateAndTimeOfOrder, 
                Price=this.Price, Quantity=Quantity,StockName=StockName,
                StockSymbol=StockSymbol,TradeAmount= (double)Price*(double)Quantity };
        }


    }
}
