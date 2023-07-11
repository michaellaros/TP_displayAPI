using DisplayOrder.Models;
using DisplayOrder.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisplayOrder.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DisplayOrderController : Controller
    {
        private readonly IDatabaseService _database;
        public DisplayOrderController(IDatabaseService database)
        {
            _database = database;
        }

        [HttpGet]
        [Route("GetOrders")]
        [ActionName("GetOrders")]
        public IActionResult GetOrders(string language)
        {
            try
            {
                return Ok(_database.GetOrdersDB(language));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("PostOrders")]
        [ActionName("PostOrders")]
        public IActionResult PostOrders(POST_OrderModel order)
        {
            try
            {
                return Ok(_database.PostOrdersDB(order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("OrderFromNav")]
        [ActionName("OrderFromNav")]
        public IActionResult OrderFromNav(IkeaOrderModel order)
        {
            try
            {
                _database.PostOrdersDB(order.GetOrderModel(), order.OrderNo);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateOrder")]
        [ActionName("UpdateOrders")]
        public IActionResult UpdateOrder(UpdateRequestModel update, string language)
        {
            try
            {
                _database.UpdateOrderDB(update, language);
                return Ok(_database.GetOrdersDB(language));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
