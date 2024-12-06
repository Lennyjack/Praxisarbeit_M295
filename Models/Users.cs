namespace Praxisarbeit_M295.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string Salt { get; set; } // Neu hinzugefügt

    public string Role { get; set; }
}


