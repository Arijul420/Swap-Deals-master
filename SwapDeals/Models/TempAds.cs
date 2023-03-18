using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SwapDeals.Models
{
    public class TempAds
    {
        //[Required]
        [DataType(DataType.Text)]
        [Display(Name = "Target Product")]
        public string TargatedProduct { get; set; }
       // [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Selling Product")]
        public string SellingProduct { get; set; }
    }
}