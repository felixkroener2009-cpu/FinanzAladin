using System.ComponentModel.DataAnnotations;

namespace FinanzAladin.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Property
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
