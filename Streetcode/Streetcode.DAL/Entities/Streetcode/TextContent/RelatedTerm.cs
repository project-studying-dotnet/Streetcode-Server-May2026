namespace Streetcode.DAL.Entities.Streetcode.TextContent
{
    public class RelatedTerm
    {
        public int Id { get; set; }
        public string? Word { get; set; }
        public int TermId { get; set; }
        public Term? Term { get; set; }
    }
}
