namespace DisplayOrder.Models
{
    public class ItemModel
    {
        public int id { get; set; }
        public string? name { get; set; }
        public int quantity { get; set; }
        public List<ItemModel> option { get; set; } = new List<ItemModel>();


    }
}
