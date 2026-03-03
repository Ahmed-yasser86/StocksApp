using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class SellOrderResponse
    {

        public Guid SellOrderID { set; get; }

        public string StockSymbol { set; get; }

        public string StockName { set; get; }

        public DateTime DateAndTimeOfOrder { set; get; }

        public uint Quantity { set; get; }

        public double Price { set; get; }

        public double TradeAmount { set; get; }
    
        }

    }
