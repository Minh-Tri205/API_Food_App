using API_Food_App.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Services.Order
{
    public class OrderService
    {
        private readonly FoodAppContext context;

        public OrderService(FoodAppContext context)
        {
            this.context = context;
        }

        // =========================
        // AUTO GENERATE ORDER CODE
        // =========================
        public string GenerateOrderCode()
        {
            return "ORD" + DateTime.Now.Ticks.ToString();
        }

        // =========================
        // CHANGE STATUS
        // =========================
        public bool ChangeStatus(int orderId, string status)
        {
            var validStatus = new List<string>
            {
                "pending",
                "confirmed",
                "preparing",
                "shipping",
                "completed",
                "cancelled"
            };

            if (!validStatus.Contains(status.ToLower()))
            {
                return false;
            }

            var order = context.Orders.Find(orderId);

            if (order == null)
            {
                return false;
            }

            order.Status = status.ToLower();

            order.UpdatedAt = DateTime.Now;

            context.SaveChanges();

            return true;
        }

        // =========================
        // CONFIRM ORDER
        // =========================
        public bool ConfirmOrder(int orderId)
        {
            return ChangeStatus(orderId, "confirmed");
        }

        // =========================
        // PREPARING ORDER
        // =========================
        public bool PreparingOrder(int orderId)
        {
            return ChangeStatus(orderId, "preparing");
        }

        // =========================
        // SHIPPING ORDER
        // =========================
        public bool ShippingOrder(int orderId)
        {
            return ChangeStatus(orderId, "shipping");
        }

        // =========================
        // COMPLETE ORDER
        // =========================
        public bool CompleteOrder(int orderId)
        {
            return ChangeStatus(orderId, "completed");
        }

        // =========================
        // CANCEL ORDER
        // =========================
        public bool CancelOrder(int orderId)
        {
            return ChangeStatus(orderId, "cancelled");
        }

        // =========================
        // CALCULATE TOTAL
        // =========================
        public decimal CalculateTotal(
            decimal subtotal,
            decimal deliveryFee,
            decimal discountAmount)
        {
            return subtotal + deliveryFee - discountAmount;
        }

        public List<API_Food_App.Models.Order>
            FetchOrdersByUserIdAndStatus(
            int userId,
            string? status)
        {
            var query = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o =>
                    o.Status.ToLower() == status.ToLower());
            }

            return query
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }
    }
}