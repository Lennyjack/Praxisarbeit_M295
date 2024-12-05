namespace Praxisarbeit_M295.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public string? Action { get; set; }
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }

        // Navigation Property
        public User? User { get; set; }
    }
}