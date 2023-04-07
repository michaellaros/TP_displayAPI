using DisplayOrder.Interfaces;
using System.Data.SqlClient;
using DisplayOrder.Models;

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
        public List<OrderModel> GetCategories(string language)
        {
            // sei arrivato     QUIIIIIII
            List<OrderModel> result = new List<OrderModel>();
            string query = @$"select distinct c.id,isnull(rct.[Value],c.[Name]) as Name,c.ImagePath from RestaurantCategory c
	                        join dbo.RestaurantCategoryTimespan ct  
		                        on ct.Category_id = c.ID 
		                        and ct.Flg_deleted = 0 
		                        and DATEPART( HOUR,GETDATE()) between ct.Available_from
                                and ct.Available_To-1
		                    left join RestaurantCategoryTranslation rct on c.ID = rct.ID
			                    and rct.Field='Name'
			                    and rct.[Language] = '{language}'";
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
                        result.Add(new OrderModel(int.Parse(reader["id"].ToString()!),
                                                     reader["name"].ToString()!,
                                                     reader["ImagePath"].ToString()!
                            ));
                    }
                }
            }
            return result;
        }
    }
}
