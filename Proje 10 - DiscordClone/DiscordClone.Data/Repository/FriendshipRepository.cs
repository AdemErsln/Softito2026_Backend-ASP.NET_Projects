using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;

namespace DiscordClone.Data.Repository
{
    public class FriendshipRepository : Repository<Friendship>, IFriendshipRepository
    {
        private readonly ApplicationDbContext _db;

        public FriendshipRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(Friendship obj)
        {
            _db.Friendships.Update(obj);
        }
    }
}
