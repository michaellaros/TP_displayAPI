using DisplayOrder.Models;
using DisplayOrder.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
        [Route("UpdateOrder")]
        [ActionName("UpdateOrders")]
        public IActionResult UpdateOrder(UpdateRequestModel update)
        {
            try
            {
                _database.UpdateOrderDB(update);
                return Ok(_database.GetOrdersDB());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
