using DisplayOrder.Models;

namespace DisplayOrder.Interfaces
{
    public interface IDatabaseService
    {

        List<OrderModel> GetOrdersDB(string language);
        int PostOrdersDB(POST_OrderModel order);
        void PostOrdersDB(POST_OrderModel order, string orderNumber);
        OrderModel UpdateOrderDB(UpdateRequestModel update, string language);

    }
}