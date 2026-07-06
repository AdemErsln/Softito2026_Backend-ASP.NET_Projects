using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordClone.Models
{
    public enum ChannelType
    {
        Text,
        Voice
    }

    public class Channel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ChannelType Type { get; set; } = ChannelType.Text;

        [Required]
        public int ServerId { get; set; }

        [ForeignKey("ServerId")]
        public virtual Server? Server { get; set; }

        // Relationships
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
