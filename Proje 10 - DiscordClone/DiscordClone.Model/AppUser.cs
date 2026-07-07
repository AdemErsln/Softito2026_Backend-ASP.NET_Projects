using Microsoft.AspNetCore.Identity;

namespace DiscordClone.Models
{
    public class AppUser : IdentityUser
    {
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public string Status { get; set; } = "Offline"; // Online, Idle, DoNotDisturb, Offline

        // Relationships
        public virtual ICollection<ServerMember> ServerMemberships { get; set; } = new List<ServerMember>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
