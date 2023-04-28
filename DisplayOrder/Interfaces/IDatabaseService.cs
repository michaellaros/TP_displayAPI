using DisplayOrder.Models;

namespace DisplayOrder.Interfaces
{
    public interface IDatabaseService
    {
        
        List<OrderModel> GetOrdersDB();
        int PostOrdersDB(List<ItemModel> items);
        OrderModel UpdateOrderDB(UpdateRequestModel update);
        
    }
}