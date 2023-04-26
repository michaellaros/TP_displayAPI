using DisplayOrder.Interfaces;
using System.Data.SqlClient;
using DisplayOrder.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Data;

namespace DisplayOrder.Services
{
    public class DatabaseService :IDatabaseService
    {
        private SqlConnection con { get; set; }

        public DatabaseService(IConfiguration configuration)
        {
            con=new SqlConnection(configuration.GetSection("appsettings").GetValue<string>("connectionstring"));
            con.Open();
        }

        public OrderModel UpdateOrderDB(UpdateRequestModel update)
        {
            string query = @$"UPDATE [dbo].[Diplay_Order]
                                 SET 
                                    [order_status] = {update.order_status},
                                    [Update_date] = GetDate()
                                    WHERE [order_id] = {update.order_id}";
            using (SqlCommand cmd = new SqlCommand(query, con))

            {
                cmd.ExecuteNonQuery();
            }
            return null;
        }
       public List<OrderModel> GetOrdersDB()
        {
            
            

            
            List<OrderModel> result = new List<OrderModel>();
            string query = @$"SELECT 
                             [order_id]
                            ,[Json_Order]
                            ,[Order_Number]
                            ,[order_status]
                            ,[Insert_date]   
                            FROM [TPDisplayDB].[dbo].[Diplay_Order]";
            using (SqlCommand cmd = new SqlCommand(query, con))

            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    while (reader.Read())
                    {

                        result.Add(new OrderModel((reader["order_id"].ToString())
                            , int.Parse(reader["Order_Number"].ToString()),
                            JsonConvert.DeserializeObject<List<ItemModel>>(reader["Json_Order"].ToString())
                            , int.Parse(reader["order_status"].ToString()), reader.GetDateTime("Insert_date").ToString("dd/MM/yyyy HH:mm:ss")
                            )) ;
                    }
                }
            }
            return result;
        }

        
        public List<OrderModel> PostOrdersDB(List<ItemModel> items)
        {
            



            List<OrderModel> result = new List<OrderModel>();
            string query = @$"INSERT INTO [dbo].[Diplay_Order](
                                
                                [Json_Order]
                                ,[Order_Number],
		                        [order_status])
                                SELECT '{JsonConvert.SerializeObject(items)}', NEXT VALUE For [dbo].[Display_OrderSequence],1";
            using (SqlCommand cmd = new SqlCommand(query, con))

            {
                cmd.ExecuteNonQuery();
            }
            return null;
        }
    }
}
