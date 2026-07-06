using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;

namespace DiscordClone.Data.Repository
{
    public class ChannelRepository : Repository<Channel>, IChannelRepository
    {
        private readonly ApplicationDbContext _db;

        public ChannelRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(Channel obj)
        {
            _db.Channels.Update(obj);
        }
    }
}
