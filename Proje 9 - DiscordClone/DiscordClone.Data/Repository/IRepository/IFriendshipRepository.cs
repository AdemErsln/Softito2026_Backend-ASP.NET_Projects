using DiscordClone.Models;

namespace DiscordClone.Data.Repository.IRepository
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        void Update(Friendship obj);
    }
}
