using System.Collections.Generic;
using System.Threading.Tasks;
using RetailOptimizationPlatform.Core.Entities;

namespace RetailOptimizationPlatform.Core.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task<IEnumerable<dynamic>> GetOrderSummariesWithRawSqlAsync();
        Task<IEnumerable<RevenueReportDto>> GetRevenueReportFromStoredProcedureAsync();
    }
}