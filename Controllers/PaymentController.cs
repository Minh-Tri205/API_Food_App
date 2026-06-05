using API_Food_App.Models;
using API_Food_App.Services.NotificationService;
using API_Food_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly FoodAppContext context;
    private readonly IConfiguration config;
    private readonly VnpayService vnpay;
    private readonly NotificationService notificationService;

    public PaymentController(
        FoodAppContext context,
        IConfiguration config,
        VnpayService vnpay,
        NotificationService notificationService)
    {
        this.context = context;
        this.config = config;
        this.vnpay = vnpay;
        this.notificationService = notificationService;
    }

    // ... GetQrInfo + GetBankInfo cu giu nguyen ...

    // ─────────────────────────────────────────────────
    // POST /api/Payment/vnpay/{orderId}
    // Tra ve { paymentUrl } cho Flutter mo trinh duyet
    // ─────────────────────────────────────────────────
    [HttpPost("vnpay/{orderId}")]
    public async Task<IActionResult> CreateVnpayUrl(int orderId)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return NotFound(new { message = "Không tìm thấy đơn hàng" });

        if (order.PaymentMethod != "vnpay")
            return BadRequest(new { message = "Đơn này không dùng VNPay" });

        var amount = (long)(order.TotalAmount ?? 0);
        var orderInfo = $"Thanh toan don hang {order.OrderCode}";
        var txnRef = order.OrderId.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString()
                 ?? "127.0.0.1";

        var url = vnpay.CreatePaymentUrl(amount, orderInfo, txnRef, ip);

        return Ok(new { paymentUrl = url });
    }

    // ─────────────────────────────────────────────────
    // GET /api/Payment/vnpay-return
    // ─────────────────────────────────────────────────
    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnpayReturn()
    {
        if (!vnpay.ValidateSignature(
                Request.Query, out var code, out var txnRef))
        {
            return BadRequest(new { message = "Chu ky khong hop le" });
        }

        if (!int.TryParse(txnRef, out var orderId))
            return BadRequest(new { message = "TxnRef khong hop le" });

        var order = await context.Orders.FindAsync(orderId);
        if (order == null)
            return NotFound(new { message = "Khong tim thay don" });

        // "00" = giao dich thanh cong
        if (code == "00")
        {
            order.Status = "confirmed";
            await context.SaveChangesAsync();

            // Push realtime notification ve Flutter qua SignalR
            await notificationService.CreateAsync(new Notification
            {
                UserId = order.UserId,
                Title = "Thanh toán VNPay thành công",
                Body = $"Đơn {order.OrderCode} đã được thanh toán.",
                NotificationType = "payment",
                RelatedId = order.OrderId
            });

            return Content(
                "<h2>✅ Thanh toán thành công. Bạn có thể quay lại app.</h2>",
                "text/html");
        }

        await notificationService.CreateAsync(new Notification
        {
            UserId = order.UserId,
            Title = "Thanh toán VNPay thất bại",
            Body = $"Đơn {order.OrderCode} chưa thanh toán được (mã {code}).",
            NotificationType = "payment",
            RelatedId = order.OrderId
        });

        return Content(
            $"<h2> Thanh toán thất bại. Mã lỗi: {code}</h2>",
            "text/html");
    }
}
