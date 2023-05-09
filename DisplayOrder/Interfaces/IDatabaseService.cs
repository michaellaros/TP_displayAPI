using DisplayOrder.Models;

namespace DisplayOrder.Interfaces
{
    public interface IDatabaseService
    {
        
        List<OrderModel> GetOrdersDB();
        int PostOrdersDB(POST_OrderModel order);
        OrderModel UpdateOrderDB(UpdateRequestModel update);
        
    }
}