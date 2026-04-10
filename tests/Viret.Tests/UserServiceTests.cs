using Moq;
using Viret.Core.Interfaces;
using Viret.Core.Models;
using Viret.Core.Services;

namespace Viret.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFamilyRepository> _familyRepositoryMock;
    private readonly Mock<IFamilyMemberRepository> _familyMemberRepositoryMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _familyRepositoryMock = new Mock<IFamilyRepository>();
        _familyMemberRepositoryMock = new Mock<IFamilyMemberRepository>();
        _sut = new UserService(_userRepositoryMock.Object, _familyRepositoryMock.Object, _familyMemberRepositoryMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_NewUser_AddsHashedPassword()
    {
        User? capturedUser = null;
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("ana@example.com")).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .Returns(Task.CompletedTask);

        await _sut.RegisterAsync("Ana", "ana@example.com", "Senha123");

        Assert.NotNull(capturedUser);
        Assert.Equal("ana@example.com", capturedUser!.Email);
        Assert.NotEqual("Senha123", capturedUser.PasswordHash);
        Assert.Contains('.', capturedUser.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("ana@example.com"))
            .ReturnsAsync(new User { Id = 1, Email = "ana@example.com" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RegisterAsync("Ana", "ana@example.com", "Senha123"));
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("ana@example.com"))
            .ReturnsAsync(new User { Id = 1, Email = "ana@example.com", PasswordHash = "invalid-hash" });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync("ana@example.com", "Senha123"));
    }

    [Fact]
    public async Task LoginAsync_WithHashFromRegister_Succeeds()
    {
        var usersByEmail = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) =>
            {
                usersByEmail.TryGetValue(email, out var user);
                return user;
            });

        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(user =>
            {
                user.Id = 7;
                usersByEmail[user.Email] = user;
            })
            .Returns(Task.CompletedTask);

        await _sut.RegisterAsync("Ana", "ana@example.com", "Senha123");
        var authenticated = await _sut.LoginAsync("ana@example.com", "Senha123");

        Assert.Equal(7, authenticated.Id);
        Assert.Equal("ana@example.com", authenticated.Email);
    }

    [Fact]
    public async Task LoginAsync_MalformedIterationsInHash_ThrowsUnauthorizedAccessException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("ana@example.com"))
            .ReturnsAsync(new User { Id = 1, Email = "ana@example.com", PasswordHash = "0.AQIDBA==.AQIDBA==" });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync("ana@example.com", "Senha123"));
    }

    [Fact]
    public async Task AddUserToFamilyAsync_DuplicateAssociation_ThrowsInvalidOperationException()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Email = "ana@example.com", PasswordHash = "hash" });
        _familyRepositoryMock.Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(new Family { Id = 2, Name = "Silva" });
        _familyMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 2))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddUserToFamilyAsync(1, 2));
    }

    [Fact]
    public async Task UserHasAccessToFamilyAsync_DelegatesToRepository()
    {
        _familyMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(true);

        var hasAccess = await _sut.UserHasAccessToFamilyAsync(1, 2);

        Assert.True(hasAccess);
    }
}
