using Canopy.Data;
using Canopy.Models;

namespace Canopy.Repositories
{
    public interface IPushSubscriptionRepository
    {
        List<PushSubscription> GetByUser(int userId);
        void Save(PushSubscription subscription);
        void DeleteByEndpoint(string endpoint);
    }

    public class PushSubscriptionRepository : IPushSubscriptionRepository
    {
        private readonly ApplicationDbContext _ctx;
        public PushSubscriptionRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public List<PushSubscription> GetByUser(int userId) =>
            _ctx.PushSubscriptions.Where(x => x.UserId == userId).ToList();

        public void Save(PushSubscription subscription)
        {
            var existing = _ctx.PushSubscriptions
                .FirstOrDefault(x => x.Endpoint == subscription.Endpoint);

            if (existing != null)
            {
                existing.P256dh = subscription.P256dh;
                existing.Auth = subscription.Auth;
            }
            else
            {
                _ctx.PushSubscriptions.Add(subscription);
            }

            _ctx.SaveChanges();
        }

        public void DeleteByEndpoint(string endpoint)
        {
            var sub = _ctx.PushSubscriptions.FirstOrDefault(x => x.Endpoint == endpoint);
            if (sub != null)
            {
                _ctx.PushSubscriptions.Remove(sub);
                _ctx.SaveChanges();
            }
        }
    }
}
