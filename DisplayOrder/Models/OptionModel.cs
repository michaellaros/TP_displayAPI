namespace DisplayOrder.Models
{
    public class OptionModel
    {
        public string name {  get; set; }
        public int quantity { get; set; }

        public OptionModel(string name, int quantity) {
            this.name = name;
            this.quantity = quantity;
        }
    }
}
