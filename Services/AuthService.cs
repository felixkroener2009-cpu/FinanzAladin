using FinanzAladin.Database;
using FinanzAladin.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzAladin.Services
{
    public class AuthService
    {
        private User? _currentUser = null;
        private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

        public event Action? OnAuthStateChanged;

        public AuthService(IDbContextFactory<FinanceDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public User? CurrentUser => _currentUser;

        public bool IsAuthenticated => _currentUser != null;

        // Register new user
        public async Task<(bool Success, string Message)> RegisterAsync(string username, string email, string password, string fullName = "")
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                // Check if user exists
                if (await context.Users.AnyAsync(u => u.Email == email || u.Username == username))
                {
                    return (false, "Benutzer mit dieser Email oder Username existiert bereits!");
                }

                // Hash password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var user = new User
                {
                    Username = username,
                    Email = email,
                    FullName = fullName,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                Console.WriteLine($"✅ Benutzer '{username}' erfolgreich registriert!");
                return (true, "Registrierung erfolgreich!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Registrierungsfehler: {ex.Message}");
                return (false, $"Registrierungsfehler: {ex.Message}");
            }
        }

        // Login user
        public async Task<(bool Success, string Message)> LoginAsync(string usernameOrEmail, string password)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

                if (user == null || !user.IsActive)
                {
                    return (false, "Benutzer nicht gefunden oder deaktiviert!");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return (false, "Falsches Passwort!");
                }

                // Update last login
                user.LastLogin = DateTime.UtcNow;
                context.Users.Update(user);
                await context.SaveChangesAsync();

                _currentUser = user;
                OnAuthStateChanged?.Invoke();

                Console.WriteLine($"✅ Benutzer '{user.Username}' erfolgreich angemeldet!");
                return (true, $"Willkommen, {user.FullName ?? user.Username}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Anmeldefehler: {ex.Message}");
                return (false, $"Anmeldefehler: {ex.Message}");
            }
        }

        // Logout user
        public void Logout()
        {
            _currentUser = null;
            OnAuthStateChanged?.Invoke();
            Console.WriteLine("✅ Benutzer abgemeldet!");
        }

        // Get user by ID
        public async Task<User?> GetUserAsync(int userId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                return await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Abrufen des Benutzers: {ex.Message}");
                return null;
            }
        }

        // Change password
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return (false, "Benutzer nicht gefunden!");
                }

                // Verify old password
                if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                {
                    return (false, "Altes Passwort ist falsch!");
                }

                // Hash new password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                context.Users.Update(user);
                await context.SaveChangesAsync();

                return (true, "Passwort erfolgreich geändert!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Ändern des Passworts: {ex.Message}");
                return (false, $"Fehler: {ex.Message}");
            }
        }
    }
}
