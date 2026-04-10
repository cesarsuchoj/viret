namespace Viret.Core.Models;

public class BudgetCategorySummary
{
    public int BudgetCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal PlannedLimit { get; set; }
    public decimal PlannedExpense { get; set; }
    public decimal ActualExpense { get; set; }
    public decimal PlannedAvailable { get; set; }
    public decimal ActualAvailable { get; set; }
}
