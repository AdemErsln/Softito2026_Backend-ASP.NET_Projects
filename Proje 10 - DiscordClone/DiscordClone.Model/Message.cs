using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordClone.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SendDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [ForeignKey("SenderId")]
        public virtual AppUser? Sender { get; set; }

        [Required]
        public int ChannelId { get; set; }

        [ForeignKey("ChannelId")]
        public virtual Channel? Channel { get; set; }
    }
}
