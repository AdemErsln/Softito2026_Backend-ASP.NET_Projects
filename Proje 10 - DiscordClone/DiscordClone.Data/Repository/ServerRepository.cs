using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;

namespace DiscordClone.Data.Repository
{
    public class ServerRepository : Repository<Server>, IServerRepository
    {
        private readonly ApplicationDbContext _db;

        public ServerRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(Server obj)
        {
            _db.Servers.Update(obj);
        }
    }
}
