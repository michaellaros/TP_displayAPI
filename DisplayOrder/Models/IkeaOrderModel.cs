namespace DisplayOrder.Models
{
    public class IkeaOrderModel
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string ReceiptRef { get; set; }
        public string OrderNo { get; set; }
        public int StoreNo { get; set; }
        public string TableNo { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string OrderReferenceCaption { get; set; }
        public List<OrderLine> OrderLines { get; set; }


        public POST_OrderModel GetOrderModel()
        {
            List<ItemModel> items = OrderLines.Select((line) => new ItemModel()
            {
                id = int.Parse(line.ItemId),
                name = line.DisplayName,
                quantity = line.Quantity
            }).ToList();
            return new POST_OrderModel(items, "DI", "");
        }
    }
    
    public class OrderLine
    {
        public int OrderId { get; set; }
        public int LineNo { get; set; }
        public string DisplayName { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public string DisplayCategoryId { get; set; }
        public string ItemId { get; set; }
        public string OrderDetail { get; set; }
        public string OrderLineType { get; set; }
    }


}
