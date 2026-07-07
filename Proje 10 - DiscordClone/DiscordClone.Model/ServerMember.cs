using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordClone.Models
{
    public class ServerMember
    {
        [Key]
        public int Id { get; set; }

        public int ServerId { get; set; }

        [ForeignKey("ServerId")]
        public virtual Server? Server { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        public string? Nickname { get; set; }

        // Server-level role, e.g., "Owner", "Admin", "Member"
        public string ServerRole { get; set; } = "Member"; 
    }
}
