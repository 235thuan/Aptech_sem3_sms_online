

namespace Sem03.Entities
{
    public class BlockUser
    {
        public int UserId { get; set; } // FK tới User
        public User User { get; set; }  // Người dùng chặn

        public int BlockedUserId { get; set; } // FK tới User bị chặn
        public User BlockedUser { get; set; }  // Người bị chặn

        public DateTime BlockAt { get; set; }
    }

}
