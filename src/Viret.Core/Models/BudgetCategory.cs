namespace Viret.Core.Models;

public class BudgetCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PlannedLimit { get; set; }

    public int FamilyId { get; set; }
    public Family? Family { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
