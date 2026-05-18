namespace Streetcode.DAL.Entities.Team
{
    public class Positions
    {
        public int Id { get; set; }
        public string? Position { get; set; }
        public List<TeamMember>? TeamMembers { get; set; }
    }
}
