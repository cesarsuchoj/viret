namespace Viret.Core.Models;

public class BudgetPeriodSummary
{
    public DateTime PeriodStart { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
    public decimal PlannedIncome { get; set; }
    public decimal ActualIncome { get; set; }
    public decimal PlannedExpense { get; set; }
    public decimal ActualExpense { get; set; }
    public decimal PlannedAvailable { get; set; }
    public decimal ActualAvailable { get; set; }
}
