namespace DisplayOrder.Models
{
    public class ItemModel
    {
        public string name { get; set; }
        public int quantity { get; set; }
        public OptionModel option { get; set; }

        public ItemModel(string name,int quantity,OptionModel option) {
            this.name = name;
            this.quantity = quantity;
            this.option = option;
        }
    }
}
