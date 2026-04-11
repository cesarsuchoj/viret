namespace Viret.Core.Models;

public class BudgetSnapshot
{
    public DateTime CapturedAt { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal PlannedIncome { get; set; }
    public decimal ActualIncome { get; set; }
    public decimal PlannedExpense { get; set; }
    public decimal ActualExpense { get; set; }
    public decimal PlannedAvailable { get; set; }
    public decimal ActualAvailable { get; set; }
}
