using DisplayOrder.Models;

namespace DisplayOrder.Interfaces
{
    public interface IDatabaseService
    {
        
        List<OrderModel> GetOrdersDB();
        
    }
}