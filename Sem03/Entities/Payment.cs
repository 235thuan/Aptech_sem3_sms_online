using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sem03.Entities
{
    public enum PaymentStatus // Enum for payment status
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically increment
        public int PaymentId { get; set; }

  
        public int UserId { get; set; }
        public virtual User User { get; set; } // Navigation property to User

        [Column(TypeName = "decimal(18, 2)")]
  
        public decimal Amount { get; set; } // Payment amount

  
        public DateTime PaymentDate { get; set; }

   
     
        public string CardNumber { get; set; }

       
        public string CardHolderName { get; set; }

    
        public DateTime ExpiryDate { get; set; }

      
        public string CVV { get; set; }

     
        public PaymentStatus PaymentStatus { get; set; }

        public string Description { get; set; } // Optional description

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current time
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Default to current time
    }

}
