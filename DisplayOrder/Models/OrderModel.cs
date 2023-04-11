using System.Collections.Generic;

namespace DisplayOrder.Models
{
    public class OrderModel
    {
    
        public int orderNumber {  get; set; }
        public List<ItemModel> items { get; set; }
        public int orderStatus { get; set; }

        public OrderModel(int orderNumber, List<ItemModel> items, int orderStatus) {

        this.orderNumber = orderNumber;
            this.items =  items;
            this.orderStatus = orderStatus;
        }
    }
}
