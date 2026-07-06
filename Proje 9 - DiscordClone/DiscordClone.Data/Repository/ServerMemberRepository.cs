using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;

namespace DiscordClone.Data.Repository
{
    public class ServerMemberRepository : Repository<ServerMember>, IServerMemberRepository
    {
        public ServerMemberRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
