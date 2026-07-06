using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;

namespace DiscordClone.Data.Repository
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
