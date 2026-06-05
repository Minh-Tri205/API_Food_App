using API_Food_App.Hubs;
using API_Food_App.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Services.NotificationService
{
    public class NotificationService
    {
        private readonly FoodAppContext context;
        private readonly IHubContext<NotificationHub> hubContext;

        public NotificationService(
            FoodAppContext context,
            IHubContext<NotificationHub> hubContext)
        {
            this.context = context;
            this.hubContext = hubContext;
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            return await context.Notifications
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetByUserAsync(int userId)
        {
            return await context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await context.Notifications
                .CountAsync(x =>
                    x.UserId == userId &&
                    x.IsRead == false);
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false;

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            await hubContext.Clients
                .Group($"user-{notification.UserId}")
                .SendAsync("ReceiveNotification", notification);

            return notification;
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notificationId);

            if (notification == null)
                throw new Exception("Notification not found");

            notification.IsRead = true;

            await context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await context.Notifications
                .Where(x =>
                    x.UserId == userId &&
                    x.IsRead == false)
                .ToListAsync();

            foreach (var item in notifications)
            {
                item.IsRead = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int notificationId)
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notificationId);

            if (notification == null)
                throw new Exception("Notification not found");

            context.Notifications.Remove(notification);

            await context.SaveChangesAsync();
        }
    }
}