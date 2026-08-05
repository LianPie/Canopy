using Canopy.Data;
using Canopy.Helpers;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public class ChatsRepository : IChatsRepository
    {
        private readonly ApplicationDbContext _ctx;
        public ChatsRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<List<int>> GetChatIdsForUserAsync(int userId)
        {
            var groupIds = _ctx.UserGroup
                .Where(ug => ug.UserId == userId)
                .Select(ug => ug.GroupId);

            return await _ctx.Chat
                .Where(c => groupIds.Contains(c.GroupId) && c.IsActive)
                .Select(c => c.Id)
                .ToListAsync();
        }
        public async Task<Chat?> GetByIdAsync(int chatId)
        {
            return await _ctx.Chat.FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<List<Chat>> GetByIdForUser(int id, int GroupId)
        {
            return _ctx.Chat.
                Include(c => c.Messages)
                .Where(c => c.GroupId == GroupId && c.Id == id)
                .ToList();
        }

        public async Task<Chat> Create(Chat chat)
        {
            _ctx.Chat.Add(chat);
            _ctx.SaveChanges();

            return chat;
        }

        public async Task<Chat> Update(Chat chat)
        {
            _ctx.Chat.Update(chat);
            _ctx.SaveChanges();

            return chat;
        }

        public async Task Delete(Chat chat)
        {
            _ctx.Chat.Remove(chat);
            _ctx.SaveChanges();

        }


    }
}
