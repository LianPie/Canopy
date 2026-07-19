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

        public List<Chat> GetByIdForUser(int id, int GroupId)
        {
            return _ctx.Chat.
                Include(c => c.Messages)
                .Where(c => c.GroupId == GroupId && c.Id == id)
                .ToList();
        }

        public Chat Create(Chat chat)
        {
            _ctx.Chat.Add(chat);
            _ctx.SaveChanges();

            return chat;
        }

        public Chat Update(Chat chat)
        {
            _ctx.Chat.Update(chat);
            _ctx.SaveChanges();

            return chat;
        }

        public void Delete(Chat chat)
        {
            _ctx.Chat.Remove(chat);
            _ctx.SaveChanges();

        }


    }
}
