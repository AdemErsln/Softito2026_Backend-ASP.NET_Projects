using System;

namespace DiscordClone.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IServerRepository Server { get; }
        IChannelRepository Channel { get; }
        IMessageRepository Message { get; }
        IServerMemberRepository ServerMember { get; }
        IFriendshipRepository Friendship { get; }
        void Save();
    }
}
