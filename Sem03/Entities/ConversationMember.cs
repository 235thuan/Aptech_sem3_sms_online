using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public class ConversationMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically incrementing
        public int Id { get; set; } // Optional: You might want to have a separate Id for this entity

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } // Navigation property to Conversation


        public int UserId { get; set; }
        public User User { get; set; } // Navigation property to User

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow; // Default to current time

        public string? RoleMember { get; set; } // Role of the user in the conversation (nullable if not always defined)
    }

}
