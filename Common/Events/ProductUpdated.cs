namespace Common.Events
{
    public class ProductUpdated : IProductEvent
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public decimal? StandardCost { get; set; }
        public decimal? ListPrice { get; set; }
        public string CategoryName { get; set; }
        public string ModelName { get; set; }
    }
}
