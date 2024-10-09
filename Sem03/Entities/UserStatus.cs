using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public class UserStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng
        public int UserStatusId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } // Quan hệ 1-1 với User
        public bool IsOnline { get; set; }
        public DateTime LastOnlineAt { get; set; }
        public bool IsPublic { get; set; }
        public DateTime StatusUpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
