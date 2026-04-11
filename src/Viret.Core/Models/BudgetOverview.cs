namespace Viret.Core.Models;

public class BudgetOverview
{
    public decimal PlannedIncome { get; set; }
    public decimal ActualIncome { get; set; }
    public decimal PlannedExpense { get; set; }
    public decimal ActualExpense { get; set; }
    public decimal PlannedAvailable { get; set; }
    public decimal ActualAvailable { get; set; }
    public IReadOnlyCollection<BudgetCategorySummary> CategorySummaries { get; set; } = Array.Empty<BudgetCategorySummary>();
}
