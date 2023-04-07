namespace DisplayOrder.Models
{
    public class OrderModel
    {
        public int orderNumber {  get; set; }
        public ItemModel items { get; set; }
        public int orderStatus { get; set; }
    }
}
