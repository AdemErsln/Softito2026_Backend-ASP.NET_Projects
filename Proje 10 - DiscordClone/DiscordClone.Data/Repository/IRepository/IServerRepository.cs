using DiscordClone.Models;

namespace DiscordClone.Data.Repository.IRepository
{
    public interface IServerRepository : IRepository<Server>
    {
        void Update(Server obj);
    }
}
