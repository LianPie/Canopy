using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Canopy.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
