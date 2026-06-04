namespace RetailOptimizationPlatform.Core.Entities
{
    /// <summary>
    /// Data Transfer Object representing inventory stock distribution by category.
    /// </summary>
    public class StockDistributionDto
    {
        public string Category { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalStock { get; set; }
    }
}
