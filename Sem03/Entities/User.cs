using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incrementing ID
        public int Id { get; set; }

     
        public int RoleId { get; set; } // Foreign key to Role
        public Role Role { get; set; } // Navigation property to Role

       
      
        public string UserName { get; set; }
        public string PasswordHash { get; set; } // Store the hashed password securely
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public Profile Profile { get; set; } // One-to-one relationship with Profile
        public UserStatus UserStatus { get; set; } // One-to-one relationship with UserStatus

        public ICollection<Friendship> Friendships { get; set; } // One-to-many relationship with Friendship
        public ICollection<BlockUser> BlockedUsers { get; set; } // One-to-many relationship with BlockUser
        public ICollection<Conversation> CreatedConversations { get; set; } // One-to-many relationship with Conversation
        public ICollection<ConversationMember> ConversationMembers { get; set; } // Many-to-many relationship with ConversationMember
        public ICollection<Payment> Payments { get; set; } // One-to-many relationship with Payment
        public ICollection<Message> Messages { get; set; } // One-to-many relationship with Message

        // Constructor to initialize collections
        public User()
        {
            Friendships = new HashSet<Friendship>();
            BlockedUsers = new HashSet<BlockUser>();
            CreatedConversations = new HashSet<Conversation>();
            ConversationMembers = new HashSet<ConversationMember>();
            Payments = new HashSet<Payment>();
            Messages = new HashSet<Message>();
        }
    }

}
