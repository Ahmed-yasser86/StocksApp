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
        string StockSymbol;

        [Required]
        string StockName;

        DateTime DateAndTimeOfOrder;
        [Range(1, 100000)]
        uint Quantity;//[Value should be between 1 and 100000]

        [Range(1, 100000)]
        double Price;//
    }
}
