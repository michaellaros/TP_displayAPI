using DisplayOrder.Models;

namespace DisplayOrder.Interfaces
{
    public interface IDatabaseService
    {

        List<OrderModel> GetOrdersDB(string language);
        int PostOrdersDB(POST_OrderModel order);
        OrderModel UpdateOrderDB(UpdateRequestModel update,string language);

    }
}