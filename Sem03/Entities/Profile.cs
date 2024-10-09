using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum MaritalStatus
    {
        Single,
        Married,
        Divorced,
        Widowed
    }

    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically incrementing
        public int Id { get; set; }

        [ForeignKey("User")] // Explicitly specifying the foreign key
        public int UserId { get; set; } // Foreign Key to User
        public User User { get; set; } // Navigation property to User

        public string FullName { get; set; } // Full name
       
        public Gender Sex { get; set; } // Gender

       
        public DateTime DOB { get; set; } // Date of Birth

        public string? Address { get; set; } // Address (optional)
        public string? Hobbies { get; set; } // Hobbies (optional)

        public MaritalStatus Marriaged { get; set; } // Marital status

        public string? Liked { get; set; } // Things liked (optional)
        public string? Disliked { get; set; } // Things disliked (optional)

        public string? Avatar { get; set; } // URL or path to the avatar image (optional)
        public string? Bio { get; set; } // Optional Bio property
        // Other attributes like Cuisine, Sport, etc.
    }

}
