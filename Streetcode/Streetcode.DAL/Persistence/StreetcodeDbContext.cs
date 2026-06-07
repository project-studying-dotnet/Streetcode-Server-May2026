using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Entities.Feedback;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Entities.Transactions;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Persistence.Configurations;

namespace Streetcode.DAL.Persistence;

public class StreetcodeDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public StreetcodeDbContext()
    {
    }

    public StreetcodeDbContext(DbContextOptions<StreetcodeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Art> Arts { get; set; } = null!;
    public DbSet<Audio> Audios { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<ToponymCoordinate> ToponymCoordinates { get; set; } = null!;
    public DbSet<StreetcodeCoordinate> StreetcodeCoordinates { get; set; } = null!;
    public DbSet<Fact> Facts { get; set; } = null!;
    public DbSet<HistoricalContext> HistoricalContexts { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<ImageDetails> ImageDetailses { get; set; } = null!;
    public DbSet<Partner> Partners { get; set; } = null!;
    public DbSet<PartnerSourceLink> PartnerSourceLinks { get; set; } = null!;
    public DbSet<RelatedFigure> RelatedFigures { get; set; } = null!;
    public DbSet<Response> Responses { get; set; } = null!;
    public DbSet<StreetcodeContent> Streetcodes { get; set; } = null!;
    public DbSet<Subtitle> Subtitles { get; set; } = null!;
    public DbSet<StatisticRecord> StatisticRecords { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<Term> Terms { get; set; } = null!;
    public DbSet<RelatedTerm> RelatedTerms { get; set; } = null!;
    public DbSet<Text> Texts { get; set; } = null!;
    public DbSet<TimelineItem> TimelineItems { get; set; } = null!;
    public DbSet<Toponym> Toponyms { get; set; } = null!;
    public DbSet<TransactionLink> TransactionLinks { get; set; } = null!;
    public DbSet<Video> Videos { get; set; } = null!;
    public DbSet<StreetcodeCategoryContent> StreetcodeCategoryContent { get; set; } = null!;
    public DbSet<StreetcodeArt> StreetcodeArts { get; set; } = null!;
    public DbSet<StreetcodeTagIndex> StreetcodeTagIndices { get; set; } = null!;
    public DbSet<TeamMember> TeamMembers { get; set; } = null!;
    public DbSet<TeamMemberLink> TeamMemberLinks { get; set; } = null!;
    public DbSet<Positions> Positions { get; set; } = null!;
    public DbSet<News> News { get; set; } = null!;
    public DbSet<SourceLinkCategory> SourceLinks { get; set; } = null!;
    public DbSet<StreetcodeImage> StreetcodeImages { get; set; } = null!;
    public DbSet<HistoricalContextTimeline> HistoricalContextsTimelines { get; set; } = null!;
    public DbSet<StreetcodePartner> StreetcodePartners { get; set; } = null!;
    public DbSet<TeamMemberPositions> TeamMemberPosition { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.UseCollation("SQL_Ukrainian_CP1251_CI_AS");

        builder.ApplyConfiguration(new StatisticRecordConfiguration());
        builder.ApplyConfiguration(new NewsConfiguration());
        builder.ApplyConfiguration(new TeamMemberConfiguration());
        builder.ApplyConfiguration(new TeamMemberLinkConfiguration());
        builder.ApplyConfiguration(new PositionsConfiguration());
        builder.ApplyConfiguration(new TagConfiguration());
        builder.ApplyConfiguration(new SubtitleConfiguration());
        builder.ApplyConfiguration(new StreetcodeTagIndexConfiguration());
        builder.ApplyConfiguration(new ToponymConfiguration());
        builder.ApplyConfiguration(new StreetcodeToponymConfiguration());
        builder.ApplyConfiguration(new PartnerConfiguration());
        builder.ApplyConfiguration(new PartnerSourceLinkConfiguration());
        builder.ApplyConfiguration(new StreetcodePartnerConfiguration());
        builder.ApplyConfiguration(new HistoricalContextConfiguration());
        builder.ApplyConfiguration(new HistoricalContextTimelineConfiguration());
        builder.ApplyConfiguration(new TimelineItemConfiguration());
        builder.ApplyConfiguration(new SourceLinkCategoryConfiguration());
        builder.ApplyConfiguration(new StreetcodeCategoryContentConfiguration());
        builder.ApplyConfiguration(new ImageConfiguration());
        builder.ApplyConfiguration(new ImageDetailsConfiguration());
        builder.ApplyConfiguration(new StreetcodeImageConfiguration());
        builder.ApplyConfiguration(new AudioConfiguration());
        builder.ApplyConfiguration(new VideoConfiguration());
        builder.ApplyConfiguration(new RelatedFigureConfiguration());
        builder.ApplyConfiguration(new StreetcodeArtConfiguration());
        builder.ApplyConfiguration(new StreetcodeContentConfiguration());
        builder.ApplyConfiguration(new PersonStreetcodeConfiguration());
        builder.ApplyConfiguration(new TextConfiguration());
        builder.ApplyConfiguration(new TermConfiguration());
        builder.ApplyConfiguration(new FactConfiguration());
        builder.ApplyConfiguration(new RelatedTermConfiguration());
        builder.ApplyConfiguration(new CoordinateConfiguration());
        builder.ApplyConfiguration(new StreetcodeCoordinateConfiguration());
        builder.ApplyConfiguration(new ToponymCoordinateConfiguration());
        builder.ApplyConfiguration(new TransactionLinkConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new ResponseConfiguration());
        builder.ApplyConfiguration(new ArtConfiguration());
        builder.ApplyConfiguration(new CommentConfiguration());
    }
}
