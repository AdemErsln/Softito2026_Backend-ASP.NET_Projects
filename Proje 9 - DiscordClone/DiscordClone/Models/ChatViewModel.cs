using DiscordClone.Models;
using System.Collections.Generic;

namespace DiscordClone.Models
{
    public class ChatVM
    {
        public IEnumerable<Server> UserServers { get; set; } = new List<Server>();
        public Server? ActiveServer { get; set; }
        public IEnumerable<Channel> Channels { get; set; } = new List<Channel>();
        public Channel? ActiveChannel { get; set; }
        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
        public IEnumerable<ServerMember> Members { get; set; } = new List<ServerMember>();
        public string CurrentUserId { get; set; } = string.Empty;
    }
}
