using CRUDOrderProducttable.Entities;
using CRUDOrderProducttable.Repositories;
using CRUDOrderProducttable.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUDOrderProducttable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        public OrderController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var order = await _orderRepo.GetOrders();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepo.GetOrderById(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            try
            {
                var result = await _orderRepo.PlaceOrder(order);

                if (result == 0)
                {
                    return StatusCode(409, "The request could not be processed because of conflict in the request");
                }
                else
                {
                    return StatusCode(200, string.Format("Record Inserted Successfuly with order Id is {0}", result));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            try
            {
                var result = await _orderRepo.UpdateOrder(order);

                if (result == 0)
                {
                    return StatusCode(409, "The request could not be processed because of conflict in the request");
                }
                else
                {
                    return StatusCode(200, string.Format("Record Updated Successfuly with order Id {0}", result));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _orderRepo.Delete(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

    }
}
