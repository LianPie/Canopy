using System.Security.Claims;
using Canopy.Hubs;
using Canopy.Models;
using Canopy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Canopy.Controllers
{
    [ApiController]
    [Route("api/Chats")]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IWebHostEnvironment _env;

        public ChatsController(IChatService chatService, IHubContext<ChatHub> chatHub, IWebHostEnvironment env)
        {
            _chatService = chatService;
            _chatHub = chatHub;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<List<int>>> GetUserChats()
        {
            var userId = GetUserId();
            var chatIds = await _chatService.GetUserChatIdsAsync(userId);
            return Ok(chatIds);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<ActionResult<List<MessageDto>>> GetMessages(
            int chatId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            var userId = GetUserId();
            var isMember = await _chatService.IsUserMemberOfChatAsync(chatId, userId);
            if (!isMember)
                return Forbid();

            var messages = await _chatService.GetMessagesAsync(chatId, skip, take);
            return Ok(messages);
        }

        [HttpPost("{chatId}/messages")]
        public async Task<ActionResult<MessageDto>> SendMessage(int chatId, [FromBody] SendMessageRequest request)
        {
            var userId = GetUserId();

            try
            {
                var dto = await _chatService.SendMessageAsync(chatId, userId, request.Text);
                return CreatedAtAction(nameof(GetMessages), new { chatId }, dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{chatId}/messages/{messageId:int}/seen")]
        public async Task<IActionResult> MarkAsSeen(int chatId, int messageId)
        {
            var userId = GetUserId();
            var isMember = await _chatService.IsUserMemberOfChatAsync(chatId, userId);
            if (!isMember)
                return Forbid();

            await _chatService.MarkMessageAsSeenAsync(messageId, userId, chatId);
            return NoContent();
        }
        [HttpPost("{chatId}/upload")]
        [RequestSizeLimit(20 * 1024 * 1024)] // 20 MB
        public async Task<IActionResult> UploadFile(int chatId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            if (file.Length > 20 * 1024 * 1024)
                return BadRequest("File too large. Max 20 MB.");

            var allowedTypes = new HashSet<string>
            {
                "image/jpeg","image/png","image/gif","image/webp","image/svg+xml",
                "video/mp4","video/webm",
                "audio/mpeg","audio/ogg","audio/wav",
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "text/plain",
                "application/zip","application/x-zip-compressed"
            };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("File type not allowed.");

            var userId = GetUserId();

            try
            {
                var dto = await _chatService.SendFileAsync(chatId, userId, file, _env.WebRootPath);
                await _chatHub.Clients.Group($"chat-{chatId}").SendAsync("ReceiveMessage", dto);
                return Ok(dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"));
        }
    }

}