namespace Viret.Core.Models;

public class Income
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public DateTime Date { get; set; }

    public int FamilyId { get; set; }
    public Family? Family { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int BudgetCategoryId { get; set; }
    public BudgetCategory? BudgetCategory { get; set; }
}
