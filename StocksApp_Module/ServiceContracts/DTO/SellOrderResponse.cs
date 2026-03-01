using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class SellOrderResponse
    {

        Guid SellOrderID;

        string StockSymbol;

        string StockName;

        DateTime DateAndTimeOfOrder;

        uint Quantity;

        double Price;

        double TradeAmount;
    }
}
