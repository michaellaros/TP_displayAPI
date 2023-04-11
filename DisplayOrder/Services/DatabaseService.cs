using DisplayOrder.Interfaces;
using System.Data.SqlClient;
using DisplayOrder.Models;
using Newtonsoft.Json;
using System.Diagnostics;

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
        public List<OrderModel> GetOrdersDB()
        {
            List<ItemModel> items = new List<ItemModel>();
            List<ItemModel> options = new List<ItemModel>();
            options.Add(new ItemModel("patatine", 2));
            items.Add(new ItemModel("hamburger",3,options));
            string test = JsonConvert.SerializeObject(items);
            Console.WriteLine(test);
            

            
            List<OrderModel> result = new List<OrderModel>();
            string query = @$"SELECT 
                            [Json_Order]
                            ,[Order_Number]
                            ,[Order_Status]
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

                        result.Add(new OrderModel(int.Parse(reader["Order_Number"].ToString()),
                            JsonConvert.DeserializeObject<List<ItemModel>>(reader["Json_Order"].ToString())
                            ,int.Parse(reader["Order_Status"].ToString()
                            )));
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
		                        [Order_Status])
                                SELECT '{JsonConvert.SerializeObject(items)}', NEXT VALUE For [dbo].[Display_OrderSequence],1";
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

                            result.Add(new OrderModel(int.Parse(reader["Order_Number"].ToString()),
                            JsonConvert.DeserializeObject<List<ItemModel>>(reader["Json_Order"].ToString())
                            , int.Parse(reader["Order_Status"].ToString()
                            )));
                    }
                }
            }
            return result;
        }
    }
}
