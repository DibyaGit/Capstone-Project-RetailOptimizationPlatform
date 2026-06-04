namespace RetailOptimizationPlatform.Core.Entities
{
    public class TopCustomerDto
    {
        public string Customer { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Total { get; set; }
    }
}
