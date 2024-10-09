using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically incrementing
        public int ConversationId { get; set; }

        public string ConversationName { get; set; } // Name of the conversation

        public bool IsGroupChat { get; set; } // Indicates if this is a group chat

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default value to current time

        [ForeignKey("Creator")] // Explicitly specify the foreign key
        public int CreatedBy { get; set; } // Foreign key to User (the creator)
        public User Creator { get; set; } // Navigation property to User (the creator)

        public ICollection<ConversationMember> ConversationMembers { get; set; } // Many-to-many relationship with ConversationMember
    }

}
