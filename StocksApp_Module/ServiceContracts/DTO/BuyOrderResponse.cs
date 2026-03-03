namespace ServiceContracts.DTO
{
    public class BuyOrderResponse
    {
      public  Guid BuyOrderID { set; get; }

        public string StockSymbol { set; get; }

        public string StockName { set; get; }

        public DateTime DateAndTimeOfOrder { set; get; }

        public uint Quantity { set; get; }

        public double Price { set; get; }

        public double TradeAmount { set; get; }



    }
}
