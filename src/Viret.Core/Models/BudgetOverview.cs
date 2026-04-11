namespace Viret.Core.Models;

public class BudgetOverview
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? FilteredUserId { get; set; }
    public decimal PlannedIncome { get; set; }
    public decimal ActualIncome { get; set; }
    public decimal PlannedExpense { get; set; }
    public decimal ActualExpense { get; set; }
    public decimal PlannedAvailable { get; set; }
    public decimal ActualAvailable { get; set; }
    public IReadOnlyCollection<BudgetCategorySummary> CategorySummaries { get; set; } = Array.Empty<BudgetCategorySummary>();
    public IReadOnlyCollection<BudgetPeriodSummary> PeriodSummaries { get; set; } = Array.Empty<BudgetPeriodSummary>();
    public IReadOnlyCollection<BudgetSnapshot> Snapshots { get; set; } = Array.Empty<BudgetSnapshot>();
}
