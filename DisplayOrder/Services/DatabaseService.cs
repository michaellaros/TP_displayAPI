using DisplayOrder.Interfaces;
using System.Data.SqlClient;
using DisplayOrder.Models;
using Newtonsoft.Json;
using System.Data;

namespace DisplayOrder.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SqlConnection con { get; set; }
        private IConfiguration _configuration { get; set; }
        public DatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public OrderModel UpdateOrderDB(UpdateRequestModel update, string language)
        {
            con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring"));

            con.Open();
            string query = @$"UPDATE [dbo].[Diplay_Order]
                                 SET 
                                    [order_status] = {update.order_status},
                                    [Update_date] = GetDate()
                                    WHERE [order_id] = {update.order_id}";
            using (SqlCommand cmd = new SqlCommand(query, con))

            {
                cmd.ExecuteNonQuery();
            }
            con.Close();
            return null;
        }
        public List<OrderModel> GetOrdersDB(string language)
        {


            con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring"));

            con.Open();
            List<OrderModel> result = new List<OrderModel>();
            string query = @$"SELECT 
                                [order_id],
                                [Json_Order],
                                [Order_Number],
                                [order_status],
                                D.[insert_date],
                                (DATEDIFF(second, D.[insert_date], GETDATE()) / 60) as result_DateMinutes,
                                (DATEDIFF(second, D.[insert_date], GETDATE()) % 60) as result_DateSeconds,
                                C.[Img] 
                            FROM [dbo].[Diplay_Order] D
                            JOIN [dbo].[Tip_ConsumationType] C 
                                ON D.Cod_Consumation = C.Cod_Consumation
                            WHERE CONVERT(date, D.[insert_date]) = CONVERT(date, GETDATE()) 
                                AND D.[order_status] < 4
                            order by d.Insert_date";
            using (SqlCommand cmd = new SqlCommand(query, con))

            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        con.Close();
                        return result;
                    }

                    while (reader.Read())
                    {
                        result.Add(new OrderModel(reader["order_id"].ToString()
                            , int.Parse(reader["Order_Number"].ToString()),
                            JsonConvert.DeserializeObject<List<ItemModel>>(reader["Json_Order"].ToString())
                            , int.Parse(reader["order_status"].ToString()),
                            reader.GetDateTime("Insert_date").ToString("dd/MM/yyyy HH:mm:ss"), int.Parse(reader["result_DateMinutes"].ToString()), int.Parse(reader["result_DateSeconds"].ToString()), reader["Img"].ToString()
                            ));
                    }
                }

                List<string> itemIds = new List<string>();
                result.ForEach(order =>
                {
                    order.items.ForEach(item =>
                    {

                        itemIds.Add(item.id.ToString());
                        item.option.ForEach(option => itemIds.Add(option.id.ToString()));

                    });
                });
                cmd.CommandText = $@"select i.id,isnull(it.Value,i.Name) name
                            from [TPKioskDB].dbo.RestaurantItem i
                            left join [TPKioskDB].dbo.RestaurantItemTranslation it on i.ID = it.ID_Item
	                            and it.Language = '{language}' and it.Field ='Name' 
                            where i.ID in({string.Join(",", itemIds)})";

                Dictionary<string, string> map = new Dictionary<string, string>();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        map.Add(reader["id"].ToString(), reader["name"].ToString());
                    }
                }
                result.ForEach(order =>
                {
                    order.items.ForEach(item =>
                    {
                        if (map.ContainsKey(item.id.ToString()))
                        {
                            item.name = map[item.id.ToString()];
                        }
                        item.option.ForEach(option =>
                        {
                            if (map.ContainsKey(option.id.ToString()))
                            {
                                option.name = map[option.id.ToString()];
                            }
                        });
                    });
                });

            }
            con.Close();
            return result;
        }

        public int PostOrdersDB(POST_OrderModel order)  // funzione chiamata dal chiosco per scrivere gli ordini sul db del display
        {
            con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring"));
            con.Open();
            int result;
            string query1 = @$"SELECT NEXT VALUE For [dbo].[Display_OrderSequence] as OrderNumberKiosk";
            string query2 = @$"INSERT INTO [dbo].[Diplay_Order](
                                [Json_Order],
                                [Order_Number],
		                        [order_status],
                                [Cod_Consumation])
                                SELECT N'{JsonConvert.SerializeObject(order.order)}', @orderNumber,1,'{order.Cod_Consumation}'";
            using (SqlCommand cmd = new SqlCommand(query1, con))

            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {

                    }

                    if (reader.Read())
                    {

                        result = int.Parse(reader["OrderNumberKiosk"].ToString()!);
                        //query2 = @$"INSERT INTO [dbo].[Diplay_Order](

                        //        [Json_Order],
                        //        [Order_Number],
                        //  [order_status],
                        //        [Cod_Consumation])
                        //        SELECT N'{JsonConvert.SerializeObject(order.order)}', {result},1,'{order.Cod_Consumation}'";

                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }


                cmd.Parameters.AddWithValue("@orderNumber", result);
                cmd.CommandText = query2;
                cmd.ExecuteNonQuery();

            }
            con.Close();
            return result;
        }


        public void PostOrdersDB(POST_OrderModel order, string orderNumber)  // funzione chiamata dal nav per scrivere gli ordini sul db del display
        {
            con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring"));
            con.Open();
            string query = @$"INSERT INTO [dbo].[Diplay_Order](
                                [Json_Order],
                                [Order_Number],
		                        [order_status],
                                [Cod_Consumation])
                                SELECT N'{JsonConvert.SerializeObject(order.order)}', @orderNumber,1,'{order.Cod_Consumation}'";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@orderNumber", orderNumber);

                int rowcount = cmd.ExecuteNonQuery();
                if (rowcount == 0) { con.Close(); throw new Exception("Couldn't insert the order!"); }

            }
            con.Close();
        }

        public void SendOrderToNav(POST_OrderModel orderModel, string orderNumber)
        {
            IkeaOrderModel order = new IkeaOrderModel()
            {
                Id = 1,//
                TransactionId = 1,//
                ReceiptRef = "1",
                OrderNo = orderNumber,
                StoreNo = 2,//
                TableNo = "",
                EmployeeId = "2", //
                EmployeeName = "POS", //check
                DateTime = DateTime.Now,
                Status = "Pending",
                OrderReferenceCaption = "",
                OrderLines = orderModel.order.Select((item, index) =>
                new OrderLine()
                {
                    OrderId = 1,
                    LineNo = index * 10000,
                    DisplayName = "", //to get in EN?
                    Quantity = item.quantity,

                }
                ).ToList()
            };
        }





    }
}
