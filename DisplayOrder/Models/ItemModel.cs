using Newtonsoft.Json;

namespace DisplayOrder.Models
{
    public class ItemModel
    {
        public string name { get; set; }
        public int quantity { get; set; }
        public List <ItemModel> option { get; set; } = new List <ItemModel> ();

        public ItemModel(string name,int quantity,List<ItemModel> option) {
            this.name = name;
            this.quantity = quantity;
            this.option = option;
        }
        public ItemModel(string name, int quantity)
        {
            this.name = name;
            this.quantity = quantity;
        }
        [JsonConstructor]
        public ItemModel()
        {

        }
    }
}
