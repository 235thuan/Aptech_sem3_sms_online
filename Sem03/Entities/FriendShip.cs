using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public enum FriendshipStatus // Enum for friendship status
    {
        Pending,
        Accepted,
        Rejected
    }

    public class Friendship
    {
        [Key, Column(Order = 0)] // Composite key part 1
        public int RequesterId { get; set; } // Foreign key to User
        public virtual User Requester { get; set; }   // Navigation property to User

        [Key, Column(Order = 1)] // Composite key part 2
        public int AccepterId { get; set; }   // Foreign key to User
        public virtual User Accepter { get; set; }    // Navigation property to User

        public FriendshipStatus Status { get; set; } // Status of the friendship

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current time

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Default to current time
    }


}
