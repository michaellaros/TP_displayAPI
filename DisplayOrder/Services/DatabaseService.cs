using DisplayOrder.Interfaces;
using System.Data.SqlClient;
using DisplayOrder.Models;
using Newtonsoft.Json;
using System.Data;
using NLog;

namespace DisplayOrder.Services
{
    public class DatabaseService : IDatabaseService
    {

        private IConfiguration _configuration { get; set; }
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public DatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void UpdateOrderDB(UpdateRequestModel update, string language)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring")))
            {
                try
                {
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
                }
                catch (Exception e)
                {
                    logger.Error($"UpdateOrderDB error: {e.Message}");
                    con.Close();
                    throw;

                }

            }
        }
        public List<OrderModel> GetOrdersDB(string language)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring")))
            {
                List<OrderModel> result = new List<OrderModel>();
                try
                {
                    con.Open();

                    string query = @$"SELECT 
                                [order_id],
                                [Json_Order],
                                [Order_Number],
                                [order_status],
                                D.[insert_date],
                                (DATEDIFF(second, D.[insert_date], GETDATE()) / 60) as result_DateMinutes,
                                (DATEDIFF(second, D.[insert_date], GETDATE()) % 60) as result_DateSeconds,
                                C.[Img],
                                [EmployeeId],
                                [EmployeeName]
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
                                result.Add(new OrderModel(
                                    reader["order_id"].ToString()!,
                                    reader["Order_Number"].ToString()!,
                                    JsonConvert.DeserializeObject<List<ItemModel>>(reader["Json_Order"].ToString()),
                                    int.Parse(reader["order_status"].ToString()!),
                                    reader.GetDateTime("Insert_date").ToString("dd/MM/yyyy HH:mm:ss"),
                                    int.Parse(reader["result_DateMinutes"].ToString()!),
                                    int.Parse(reader["result_DateSeconds"].ToString()!),
                                    reader["Img"].ToString()!,
                                    reader["EmployeeId"].ToString()!,
                                    reader["EmployeeName"].ToString()!
                                    ));
                            }
                        }

                        List<string> itemIds = new List<string>();
                        result.ForEach(order =>
                        {
                            order.items.Where(item => item.name == null).ToList().ForEach(item =>
                            {

                                itemIds.Add(item.id.ToString());
                                item.option.ForEach(option => itemIds.Add(option.id.ToString()));

                            });
                        });
                        if (itemIds.Count > 0)
                        {
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
                                    map.Add(reader["id"].ToString()!,
                                        reader["name"].ToString()!);
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


                    }
                    con.Close();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Error($"GetOrdersDB error: {e.Message}");
                    con.Close();
                    throw;

                }
            }

        }

        public int PostOrdersDB(POST_OrderModel order)  // funzione chiamata dal chiosco per scrivere gli ordini sul db del display
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring")))
            {
                int result;

                try
                {
                    con.Open();

                    //string query1 = @$"SELECT NEXT VALUE For [dbo].[Display_OrderSequence] as OrderNumberKiosk";
                    string query = @$"INSERT INTO [dbo].[Diplay_Order](
                                            [Json_Order],
                                            [Order_Number],
		                                    [order_status],
                                            [Cod_Consumation],
                                            [EmployeeId],
                                            [EmployeeName])
                                        output INSERTED.Order_Number
                                    values
                                            (@order, cast (NEXT VALUE For [dbo].[Display_OrderSequence] as INT),1,@cod_consumation,@id,'SCO')";



                    using (SqlCommand cmd = new SqlCommand(query, con))

                    {


                        //using (SqlDataReader reader = cmd.ExecuteReader())
                        //{


                        //        result = int.Parse(reader["OrderNumberKiosk"].ToString()!);
                        //        //query2 = @$"INSERT INTO [dbo].[Diplay_Order](

                        //        //        [Json_Order],
                        //        //        [Order_Number],
                        //        //  [order_status],
                        //        //        [Cod_Consumation])
                        //        //        SELECT N'{JsonConvert.SerializeObject(order.order)}', {result},1,'{order.Cod_Consumation}'";

                        //    }
                        //    else
                        //    {
                        //        throw new ArgumentException();
                        //    }

                        //}


                        cmd.Parameters.AddWithValue("@order", JsonConvert.SerializeObject(order.order));
                        cmd.Parameters.AddWithValue("@cod_consumation", order.Cod_Consumation);
                        cmd.Parameters.AddWithValue("@id", order.kioskId);

                        var orderNumber = cmd.ExecuteScalar();
                        if (orderNumber == null) { con.Close(); throw new Exception("Couldn't insert the new order!"); }
                        result = int.Parse(orderNumber.ToString());

                    }
                    con.Close();
                    return result;


                }
                catch (Exception e)
                {
                    logger.Error($"PostOrdersDB kiosk error: {e.Message}");
                    con.Close();
                    throw;

                }
            }
        }


        public void PostOrdersDB(POST_OrderModel order, string orderNumber)  // funzione chiamata dal nav per scrivere gli ordini sul db del display
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetSection("appsettings").GetValue<string>("connectionstring")))
            {
                try
                {
                    con.Open();
                    string query = @$"INSERT INTO [dbo].[Diplay_Order](
                                [Json_Order],
                                [Order_Number],
		                        [order_status],
                                [Cod_Consumation],
                                [EmployeeId],
                                [EmployeeName])
                                SELECT @json, @orderNumber,1,'{order.Cod_Consumation}',@id,'POS'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@orderNumber", orderNumber);
                        cmd.Parameters.AddWithValue("@id", order.kioskId);
                        cmd.Parameters.AddWithValue("@json", JsonConvert.SerializeObject(order.order));

                        int rowcount = cmd.ExecuteNonQuery();
                        if (rowcount == 0) { con.Close(); throw new Exception("Couldn't insert the order!"); }

                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    logger.Error($"PostOrdersDB nav error: {e.Message}");
                    con.Close();
                    throw;

                }

            }
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
