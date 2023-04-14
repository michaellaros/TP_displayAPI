namespace DisplayOrder.Models
{
    public class UpdateRequestModel
    {
        public string order_id { get; set; }
        public int order_status { get; set; }
        public UpdateRequestModel(string order_id, int order_status)
        {
            this.order_id = order_id;
            this.order_status = order_status;
        }
    }
}
