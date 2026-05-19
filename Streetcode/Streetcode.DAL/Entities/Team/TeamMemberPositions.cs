namespace Streetcode.DAL.Entities.Team
{
    public class TeamMemberPositions
    {
        public int TeamMemberId { get; set; }
        public Positions Positions { get; set; }
        public TeamMember TeamMember { get; set; }
        public int PositionsId { get; set; }
    }
}
