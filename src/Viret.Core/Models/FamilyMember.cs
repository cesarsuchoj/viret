namespace Viret.Core.Models;

public class FamilyMember
{
    public int FamilyId { get; set; }
    public Family? Family { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public FamilyRole Role { get; set; } = FamilyRole.Member;
}
