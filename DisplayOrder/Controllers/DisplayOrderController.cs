using DisplayOrder.Models;
using DisplayOrder.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisplayOrder.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DisplayOrderController : Controller
    {
        IDatabaseService _database;
        public DisplayOrderController(IConfiguration configuration, IDatabaseService database)
        {
            _database = database;
        }

        [HttpGet]
        [Route("Orders")]
        [ActionName("GetOrders")]
        public IActionResult GetOrders()
        {
            try
            {
                return Ok(_database.GetOrdersDB());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("Orders")]
        [ActionName("PostOrders")]
        public IActionResult PostOrders(List<ItemModel> items)
        {
            try
            {
                return Ok(_database.PostOrdersDB(items));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
