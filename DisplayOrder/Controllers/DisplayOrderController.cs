using DisplayOrder.Models;
using DisplayOrder.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DisplayOrder.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DisplayOrderController : Controller
    {
        private readonly IDatabaseService _database;
        private ILogger<IDatabaseService> _logger;
        public DisplayOrderController(IDatabaseService database, ILogger<IDatabaseService> logger)
        {
            _database = database;
            this._logger = logger;
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
                _logger.LogInformation($"Order from kiosk: {JsonConvert.SerializeObject(order)}");
                return Ok(_database.PostOrdersDB(order));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order from kiosk error: {ex.Message}");
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
                _logger.LogInformation($"Order from nav: {JsonConvert.SerializeObject(order)}");
                _database.PostOrdersDB(order.GetOrderModel(), order.OrderNo);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order from nav error: {ex.Message}");
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
                _logger.LogInformation($"Update order: {JsonConvert.SerializeObject(update)}");
                _database.UpdateOrderDB(update, language);
                return Ok(_database.GetOrdersDB(language));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update order error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
