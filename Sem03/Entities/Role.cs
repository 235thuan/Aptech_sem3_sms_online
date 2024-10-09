using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng
        public int RoleId { get; set; }

        public string Name { get; set; } // Tên vai trò (Admin, User, Manager, etc.)

        public ICollection<User> Users { get; set; } // Quan hệ 1-n với User
    }

}
