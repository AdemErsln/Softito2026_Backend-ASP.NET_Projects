using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordClone.Models
{
    public class Server
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        public string? Photo { get; set; }

        [Required]
        public string InvitationCode { get; set; } = string.Empty;

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [ForeignKey("OwnerId")]
        public virtual AppUser? Owner { get; set; }

        // Relationships
        public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();
        public virtual ICollection<ServerMember> Members { get; set; } = new List<ServerMember>();
    }
}
