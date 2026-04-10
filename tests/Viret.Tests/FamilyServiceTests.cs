using Moq;
using Viret.Core.Interfaces;
using Viret.Core.Models;
using Viret.Core.Services;

namespace Viret.Tests;

public class FamilyServiceTests
{
    private readonly Mock<IFamilyRepository> _repositoryMock;
    private readonly FamilyService _sut;

    public FamilyServiceTests()
    {
        _repositoryMock = new Mock<IFamilyRepository>();
        _sut = new FamilyService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateFamilyAsync_ValidFamily_CallsRepository()
    {
        var family = new Family { Name = "Silva" };

        await _sut.CreateFamilyAsync(family);

        _repositoryMock.Verify(r => r.AddAsync(family), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateFamilyAsync_EmptyName_ThrowsArgumentException(string? name)
    {
        var family = new Family { Name = name! };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateFamilyAsync(family));
    }

    [Fact]
    public async Task GetAllFamiliesAsync_DelegatesToRepository()
    {
        var expected = new List<Family> { new() { Id = 1, Name = "Silva" } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _sut.GetAllFamiliesAsync();

        Assert.Equal(expected, result);
    }
}
