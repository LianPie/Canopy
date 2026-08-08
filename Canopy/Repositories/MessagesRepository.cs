using Canopy.Data;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;

namespace Canopy.Repositories
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly ApplicationDbContext _ctx;
        public MessagesRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<List<Message>> GetByChatIdAsync(int chatId, int skip = 0, int take = 50)
        {
            return await _ctx.Message
                .Where(m => m.ChatId == chatId)
                .Include(m => m.User)
                .Include(m => m.MessageAttachments)
                .OrderByDescending(m => m.DateCreated)
                .Skip(skip)
                .Take(take)
                .OrderBy(m => m.DateCreated) 
                .ToListAsync();
        }

        public async Task<Message> CreateAsync(Message message)
        {
            _ctx.Message.Add(message);
            await _ctx.SaveChangesAsync(); 

            await _ctx.Entry(message).Reference(m => m.User).LoadAsync();

            return message;
        }

        public async Task MarkAsSeenAsync(int messageId, int userId)
        {
            var exists = await _ctx.MessageSeenStatus
                .AnyAsync(s => s.MessageId == messageId && s.UserId == userId);
            if (exists) return;

            _ctx.MessageSeenStatus.Add(new MessageSeenStatus
            {
                MessageId = messageId,
                UserId = userId,
                SeenDate = DateTime.Now
            });
            await _ctx.SaveChangesAsync();
        }
    }
}
