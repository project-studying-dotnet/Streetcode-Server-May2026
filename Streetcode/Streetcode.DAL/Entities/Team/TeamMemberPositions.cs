namespace Streetcode.DAL.Entities.Team
{
    public class TeamMemberPositions
    {
        public int TeamMemberId { get; set; }
        public Positions Positions { get; set; } = null!;
        public TeamMember TeamMember { get; set; } = null!;
        public int PositionsId { get; set; }
    }
}
