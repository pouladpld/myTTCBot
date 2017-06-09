using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTTCBot.Models
{
    [Table("frequent_location")]
    public class FrequentLocation
    {
        [ForeignKey(nameof(UserChatContextId))]
        public UserChatContext UserChatContext { get; set; }

        [Key]
        [Column("userchat_context_id")]
        public int UserChatContextId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("name")]
        public string Name { get; set; }

        [Column("lat")]
        public double Latitude { get; set; }

        [Column("lon")]
        public double Longitude { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
