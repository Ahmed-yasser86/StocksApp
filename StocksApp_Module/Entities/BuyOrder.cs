using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class BuyOrder
    {

        [Key]
        Guid BuyOrderID;
        [Required]
        string StockSymbol;
        
        [Required]
        string StockName;
        
        DateTime DateAndTimeOfOrder;
        [Range(1,100000)]
        uint Quantity;//[Value should be between 1 and 100000]
       
        [Range(1,100000)]
        double Price;//
    }
}
