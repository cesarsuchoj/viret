using Moq;
using Viret.Core.Interfaces;
using Viret.Core.Models;
using Viret.Core.Services;

namespace Viret.Tests;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _repositoryMock;
    private readonly TransactionService _sut;

    public TransactionServiceTests()
    {
        _repositoryMock = new Mock<ITransactionRepository>();
        _sut = new TransactionService(_repositoryMock.Object);
    }

    [Fact]
    public async Task AddTransactionAsync_ValidTransaction_CallsRepository()
    {
        var transaction = new Transaction
        {
            Description = "Salary",
            Amount = 5000m,
            Date = DateTime.Today,
            Type = TransactionType.Income,
            FamilyId = 1
        };

        await _sut.AddTransactionAsync(transaction);

        _repositoryMock.Verify(r => r.AddAsync(transaction), Times.Once);
    }

    [Fact]
    public async Task AddTransactionAsync_ZeroAmount_ThrowsArgumentException()
    {
        var transaction = new Transaction
        {
            Description = "Invalid",
            Amount = 0m,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            FamilyId = 1
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddTransactionAsync(transaction));
    }

    [Fact]
    public async Task AddTransactionAsync_NegativeAmount_ThrowsArgumentException()
    {
        var transaction = new Transaction
        {
            Description = "Invalid",
            Amount = -100m,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            FamilyId = 1
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddTransactionAsync(transaction));
    }

    [Fact]
    public async Task GetBalanceAsync_DelegatesToRepository()
    {
        const int familyId = 1;
        _repositoryMock.Setup(r => r.GetBalanceByFamilyIdAsync(familyId)).ReturnsAsync(1500m);

        var balance = await _sut.GetBalanceAsync(familyId);

        Assert.Equal(1500m, balance);
    }

    [Fact]
    public async Task GetTransactionsByFamilyAsync_DelegatesToRepository()
    {
        const int familyId = 1;
        var expected = new List<Transaction> { new() { Id = 1, FamilyId = familyId } };
        _repositoryMock.Setup(r => r.GetByFamilyIdAsync(familyId)).ReturnsAsync(expected);

        var result = await _sut.GetTransactionsByFamilyAsync(familyId);

        Assert.Equal(expected, result);
    }
}
