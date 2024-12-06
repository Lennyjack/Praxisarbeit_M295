using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Models;

namespace Praxisarbeit_M295.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet für jede Tabelle
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguration für User-Entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId); // Primärschlüssel
                entity.Property(u => u.Username).IsRequired(); // Benutzername ist Pflichtfeld
                entity.Property(u => u.PasswordHash).IsRequired(); // Passwort-Hash ist Pflichtfeld
                entity.Property(u => u.Salt).IsRequired(); // Salt ist Pflichtfeld
                entity.Property(u => u.Role).IsRequired(); // Rolle ist Pflichtfeld
            });

            // Konfiguration für ServiceOrder-Entity (Beispiel)
            modelBuilder.Entity<ServiceOrder>(entity =>
            {
                entity.HasKey(so => so.OrderId); // Primärschlüssel
                entity.Property(so => so.Service).IsRequired();
                entity.Property(so => so.Priority).IsRequired();
                entity.Property(so => so.Status).IsRequired();
                // Beziehungen, wenn nötig
            });

            // Konfiguration für Log-Entity (Beispiel)
            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(l => l.LogId); // Primärschlüssel
                entity.Property(l => l.Action).IsRequired();
                entity.Property(l => l.Timestamp).IsRequired();
                // Weitere Eigenschaften, falls notwendig
            });

            // Seed-Daten für User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    PasswordHash = CreatePasswordHash("admin123", out var salt),
                    Salt = salt,
                    Role = "Admin"
                }
            );
        }

        // Helper-Methode zum Generieren von Passwort-Hash und Salt für Seed-Daten
        private static string CreatePasswordHash(string password, out string salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256())
            {
                salt = Convert.ToBase64String(hmac.Key); // Salt generieren
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash); // Passwort-Hash zurückgeben
            }
        }
    }
}
