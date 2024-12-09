using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Praxisarbeit_M295.Models
{
    public class ServiceOrder
    {
        [Key]
        public int OrderId { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        [Required]
        public string? Priority { get; set; }

        [Required]
        public string? Service { get; set; }

        [Required]
        public string? Status { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Fremdschlüssel zu User
        [Required]
        public int AssignedTo { get; set; } // Fremdschlüssel

        [ForeignKey(nameof(AssignedTo))] // Verknüpfung mit AssignedTo
        public User? AssignedUser { get; set; } // Navigation Property
    }
}
