using DisplayOrder.Models;

namespace DisplayOrder.Interfaces
{
    public interface IDatabaseService
    {
        
        List<OrderModel> GetOrdersDB();
        List<OrderModel> PostOrdersDB(List<ItemModel> items);
        OrderModel UpdateOrderDB(UpdateRequestModel update);
        
    }
}