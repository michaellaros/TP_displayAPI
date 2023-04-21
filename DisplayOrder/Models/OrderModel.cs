
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DisplayOrder.Models
{
    public class OrderModel
    {
        public string order_id { get; set; }
        public int orderNumber {  get; set; }
        public List<ItemModel> items { get; set; }
        public int order_status { get; set; }
        [JsonIgnore]
        public string Insert_date { get; set; }
        public string date { get{ return Insert_date; } }


        public OrderModel(string order_id, int orderNumber, List<ItemModel> items, int order_status, string Insert_date) {
            this.order_id = order_id;
            this.orderNumber = orderNumber;
            this.items =  items;
            this.order_status = order_status;
            this.Insert_date = Insert_date;

        }
    }
}
