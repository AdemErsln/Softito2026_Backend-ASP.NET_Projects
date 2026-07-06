using DiscordClone.Data.Repository.IRepository;
using System;

namespace DiscordClone.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Server = new ServerRepository(_context);
            Channel = new ChannelRepository(_context);
            Message = new MessageRepository(_context);
            ServerMember = new ServerMemberRepository(_context);
            Friendship = new FriendshipRepository(_context);
        }

        public IServerRepository Server { get; private set; }
        public IChannelRepository Channel { get; private set; }
        public IMessageRepository Message { get; private set; }
        public IServerMemberRepository ServerMember { get; private set; }
        public IFriendshipRepository Friendship { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
