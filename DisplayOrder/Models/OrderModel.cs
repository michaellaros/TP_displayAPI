using System.Collections.Generic;

namespace DisplayOrder.Models
{
    public class OrderModel
    {
        public string order_id { get; set; }
        public int orderNumber {  get; set; }
        public List<ItemModel> items { get; set; }
        public int order_status { get; set; }

        public OrderModel(string order_id, int orderNumber, List<ItemModel> items, int order_status) {
            this.order_id = order_id;
            this.orderNumber = orderNumber;
            this.items =  items;
            this.order_status = order_status;
        }
    }
}
