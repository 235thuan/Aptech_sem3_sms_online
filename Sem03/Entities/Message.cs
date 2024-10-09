using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public enum MessageType // Enum for message type
    {
        Text,
        Image,
        File
    }

    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically increment
        public int MessageId { get; set; }

        public int ConversationId { get; set; }
        public virtual Conversation Conversation { get; set; } // Navigation property to Conversation

      
        public int SenderId { get; set; }
        public virtual User Sender { get; set; } // Navigation property to User (sender)

      
        public string Content { get; set; }

        public MessageType MessageType { get; set; } // Enum for message type

      
        public DateTime SendAt { get; set; } = DateTime.UtcNow; // Default to current time

        public DateTime? EditAt { get; set; } // Nullable edit timestamp

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>(); // Relationship 1-n with Attachment
    }

}
