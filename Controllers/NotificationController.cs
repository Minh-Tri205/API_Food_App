using API_Food_App.DTOs;
using API_Food_App.Models;
using API_Food_App.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService notificationService;

        public NotificationController(
            NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await notificationService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await notificationService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var result =
                await notificationService.GetByUserAsync(userId);

            return Ok(result);
        }

        [HttpGet("unread-count/{userId}")]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var count =
                await notificationService.GetUnreadCountAsync(userId);

            return Ok(new
            {
                unreadCount = count
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] Notification notification)
        {
            var result =
                await notificationService.CreateAsync(notification);

            return Ok(result);
        }

        [HttpPatch("read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await notificationService.MarkAsReadAsync(id);

            return Ok(new
            {
                message = "Notification marked as read"
            });
        }

        [HttpPatch("read-all/{userId}")]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            await notificationService.MarkAllAsReadAsync(userId);

            return Ok(new
            {
                message = "All notifications marked as read"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await notificationService.DeleteAsync(id);

            return Ok(new
            {
                message = "Notification deleted"
            });
        }

        [HttpPost("test-send")]
        public async Task<IActionResult> TestSend(
            [FromBody] TestNotificationRequestDto request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Body = request.Body,
                NotificationType = "order"
            };

            var result =
                await notificationService.CreateAsync(notification);

            return Ok(result);
        }
    }
}