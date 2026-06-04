namespace RetailOptimizationPlatform.Core.Entities
{
    /// <summary>
    /// Data Transfer Object representing the database stored procedure revenue report output.
    /// </summary>
    public class RevenueReportDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenueGenerated { get; set; }
    }
}
