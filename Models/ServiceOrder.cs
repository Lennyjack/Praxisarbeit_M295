namespace Praxisarbeit_M295.Models
{
    public class ServiceOrder
    {
        public int OrderId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Priority { get; set; }
        public string? Service { get; set; }
        public string? Status { get; set; }
        public int? AssignedTo { get; set; }

        // Navigation Property
        public User? AssignedUser { get; set; }
    }
}
