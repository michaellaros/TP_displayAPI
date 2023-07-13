namespace DisplayOrder.Models
{
    public class POST_OrderModel
    {
        public List<ItemModel> order { get; set; }
        public string Cod_Consumation { get; set; }
        public string kioskId { get; set; }
        //public POST_OrderModel(List<ItemModel> order, string Cod_Consumation, string kioskIp)
        //{
        //    this.order = order;
        //    this.Cod_Consumation = Cod_Consumation;
        //    this.kioskIp = kioskIp;
        //}
    }
}
