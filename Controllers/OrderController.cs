using API_Food_App.Models;
using API_Food_App.Services.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly FoodAppContext context;

        private readonly OrderService orderService;

        public OrderController(
            FoodAppContext context,
            OrderService orderService)
        {
            this.context = context;
            this.orderService = orderService;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return Ok(orders);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderTrackings)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // =========================
        // GET BY USER ID
        // =========================
        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(int userId)
        {
            var orders = context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return Ok(orders);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(Order order)
        {
            order.OrderCode =
                orderService.GenerateOrderCode();

            order.Status = "pending";

            order.CreatedAt = DateTime.Now;

            order.UpdatedAt = DateTime.Now;

            // DEFAULT VALUE
            order.DeliveryFee ??= 0;

            order.DiscountAmount ??= 0;

            order.TotalAmount ??= 0;

            context.Orders.Add(order);

            context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.OrderId },
                order
            );
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPatch("{id}")]
        public IActionResult Update(int id, Order dto)
        {
            var order = context.Orders.Find(id);

            if (order == null)
            {
                return NotFound();
            }

            // RecipientName
            if (!string.IsNullOrWhiteSpace(dto.RecipientName))
            {
                order.RecipientName = dto.RecipientName;
            }

            // DeliveryAddress
            if (!string.IsNullOrWhiteSpace(dto.DeliveryAddress))
            {
                order.DeliveryAddress = dto.DeliveryAddress;
            }

            // DeliveryPhone
            if (!string.IsNullOrWhiteSpace(dto.DeliveryPhone))
            {
                order.DeliveryPhone = dto.DeliveryPhone;
            }

            // DeliveryFee
            if (dto.DeliveryFee.HasValue)
            {
                order.DeliveryFee = dto.DeliveryFee.Value;
            }

            // Note
            if (!string.IsNullOrWhiteSpace(dto.Note))
            {
                order.Note = dto.Note;
            }

            // AddressId
            if (dto.AddressId.HasValue)
            {
                order.AddressId = dto.AddressId.Value;
            }

            // PaymentMethod
            if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                order.PaymentMethod = dto.PaymentMethod;
            }

            // VoucherId
            if (dto.VoucherId.HasValue)
            {
                order.VoucherId = dto.VoucherId.Value;
            }

            // DiscountAmount
            if (dto.DiscountAmount.HasValue)
            {
                order.DiscountAmount = dto.DiscountAmount.Value;
            }

            // TotalAmount
            if (dto.TotalAmount.HasValue)
            {
                order.TotalAmount = dto.TotalAmount.Value;
            }

            // Status
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                order.Status = dto.Status;
            }

            order.UpdatedAt = DateTime.Now;

            context.SaveChanges();

            return Ok(order);
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = context.Orders.Find(id);

            if (order == null)
            {
                return NotFound();
            }

            context.Orders.Remove(order);

            context.SaveChanges();

            return NoContent();
        }

        // =========================
        // CHANGE STATUS
        // =========================
        [HttpPatch("change-status/{id}")]
        public IActionResult ChangeStatus(
            int id,
            [FromBody] string status)
        {
            var result =
                orderService.ChangeStatus(id, status);

            if (!result)
            {
                return BadRequest(
                    "Invalid status or order not found");
            }

            return Ok("Status updated");
        }

        // =========================
        // CONFIRM ORDER
        // =========================
        [HttpPatch("confirm/{id}")]
        public IActionResult ConfirmOrder(int id)
        {
            var result =
                orderService.ConfirmOrder(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Order confirmed");
        }

        // =========================
        // PREPARING ORDER
        // =========================
        [HttpPatch("preparing/{id}")]
        public IActionResult PreparingOrder(int id)
        {
            var result =
                orderService.PreparingOrder(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Order preparing");
        }

        // =========================
        // SHIPPING ORDER
        // =========================
        [HttpPatch("shipping/{id}")]
        public IActionResult ShippingOrder(int id)
        {
            var result =
                orderService.ShippingOrder(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Order shipping");
        }

        // =========================
        // COMPLETE ORDER
        // =========================
        [HttpPatch("complete/{id}")]
        public IActionResult CompleteOrder(int id)
        {
            var result =
                orderService.CompleteOrder(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Order completed");
        }

        // =========================
        // CANCEL ORDER
        // =========================
        [HttpPatch("cancel/{id}")]
        public IActionResult CancelOrder(int id)
        {
            var result =
                orderService.CancelOrder(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Order cancelled");
        }

        // =========================
        // GET BY USER ID + STATUS
        // =========================
        [HttpGet("user/{userId}/status")]
        public IActionResult FetchOrdersByUserIdAndStatus(int userId, [FromQuery] string? status)
        {
            var orders =
                orderService.FetchOrdersByUserIdAndStatus(
                    userId,
                    status);

            return Ok(orders);
        }
    }
}