namespace RetailOptimizationPlatform.Core.Entities
{
    /// <summary>
    /// Data Transfer Object representing sales trend over time.
    /// </summary>
    public class SalesTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal MonthlySales { get; set; }
        public int OrderCount { get; set; }
    }
}
