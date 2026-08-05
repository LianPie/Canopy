using System.Security.Claims;
using Canopy.Models;
using Canopy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Canopy.Controllers
{
    [ApiController]
    [Route("api/Chats")]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
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
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"));
        }
    }

}