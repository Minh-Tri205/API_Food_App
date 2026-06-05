using Microsoft.AspNetCore.SignalR;

namespace API_Food_App.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Flutter ket noi: /hubs/notification?userId=123
            var httpContext = Context.GetHttpContext();
            var userIdStr = httpContext?.Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out var uid))
            {
                // Join group rieng cua user nay
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    $"user-{uid}"
                );
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userIdStr = httpContext?.Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out var uid))
            {
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId,
                    $"user-{uid}"
                );
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}