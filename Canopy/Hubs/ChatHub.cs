using Canopy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Canopy.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        public ChatHub(IChatService chatService) => _chatService = chatService;

        private int GetUserId() =>
            int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var chatIds = await _chatService.GetUserChatIdsAsync(userId);

            foreach (var chatId in chatIds)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(int chatId, string text)
        {
            var userId = GetUserId(); // نه از پارامتر کلاینت
            var dto = await _chatService.SendMessageAsync(chatId, userId, text);
            await Clients.Group($"chat-{chatId}").SendAsync("ReceiveMessage", dto);
        }
    }
}
