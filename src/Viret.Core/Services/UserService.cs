using System.Security.Cryptography;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Core.Services;

public class UserService : IUserService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private const int MinIterations = 10_000;
    private const int MaxIterations = 1_000_000;

    private readonly IUserRepository _userRepository;
    private readonly IFamilyRepository _familyRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;

    public UserService(
        IUserRepository userRepository,
        IFamilyRepository familyRepository,
        IFamilyMemberRepository familyMemberRepository)
    {
        _userRepository = userRepository;
        _familyRepository = familyRepository;
        _familyMemberRepository = familyMemberRepository;
    }

    public async Task<User> RegisterAsync(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("Password must contain at least 8 characters.", nameof(password));

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existingUser is not null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordHash = HashPassword(password)
        };

        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user is null || !VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return user;
    }

    public async Task<IEnumerable<Family>> GetFamiliesForUserAsync(int userId)
    {
        await EnsureUserExistsAsync(userId);
        return await _familyMemberRepository.GetFamiliesByUserIdAsync(userId);
    }

    public async Task AddUserToFamilyAsync(int userId, int familyId, FamilyRole role = FamilyRole.Member)
    {
        await EnsureUserExistsAsync(userId);
        await EnsureFamilyExistsAsync(familyId);

        if (await _familyMemberRepository.ExistsAsync(userId, familyId))
            throw new InvalidOperationException("User is already associated with this family.");

        await _familyMemberRepository.AddAsync(new FamilyMember
        {
            UserId = userId,
            FamilyId = familyId,
            Role = role
        });
    }

    public Task<bool> UserHasAccessToFamilyAsync(int userId, int familyId)
        => _familyMemberRepository.ExistsAsync(userId, familyId);

    private async Task EnsureUserExistsAsync(int userId)
    {
        if (await _userRepository.GetByIdAsync(userId) is null)
            throw new KeyNotFoundException("User not found.");
    }

    private async Task EnsureFamilyExistsAsync(int familyId)
    {
        if (await _familyRepository.GetByIdAsync(familyId) is null)
            throw new KeyNotFoundException("Family not found.");
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;
        if (iterations < MinIterations || iterations > MaxIterations)
            return false;

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            expectedHash = Convert.FromBase64String(parts[2]);
            if (salt.Length == 0 || expectedHash.Length == 0)
                return false;
        }
        catch (FormatException)
        {
            return false;
        }

        try
        {
            var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}
