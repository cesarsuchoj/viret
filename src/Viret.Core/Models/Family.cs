namespace Viret.Core.Models;

public class Family
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<FamilyMember> Members { get; set; } = new List<FamilyMember>();
}
