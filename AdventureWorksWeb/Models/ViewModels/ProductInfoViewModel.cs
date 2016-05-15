using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdventureWorksWeb.Models.ViewModels
{
    public class ProductInfoViewModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public string Color { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string Size { get; set; }
        public decimal? Weight { get; set; }
        public string CategoryName { get; set; }
        public string ModelName { get; set; }
        public string ThumbnailPhoto { get; set; }
    }
}