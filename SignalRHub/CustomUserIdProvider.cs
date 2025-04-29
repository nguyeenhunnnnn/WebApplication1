using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
namespace WebApplication1.SignalRHub
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Lấy Claim NameIdentifier làm UserId trong SignalR
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
