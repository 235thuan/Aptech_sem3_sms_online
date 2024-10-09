using Sem03.Entities;

namespace Sem03.DTOs
{
    public class FriendshipDto
    {
        public int RequesterId { get; set; }
        public int AccepterId { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

