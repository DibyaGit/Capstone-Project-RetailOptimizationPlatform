namespace RetailOptimizationPlatform.Core.Entities
{
    public class TopSellingItemDto
    {
        public int InventoryId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalSales { get; set; }
    }
}
