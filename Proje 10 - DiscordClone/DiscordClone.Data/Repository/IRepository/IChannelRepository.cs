using DiscordClone.Models;

namespace DiscordClone.Data.Repository.IRepository
{
    public interface IChannelRepository : IRepository<Channel>
    {
        void Update(Channel obj);
    }
}
