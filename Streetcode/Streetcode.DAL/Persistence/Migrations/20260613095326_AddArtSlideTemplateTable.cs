using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streetcode.DAL.Persistence.Migrations
{
    public partial class AddArtSlideTemplateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "streetcode_art_slide_template",
                schema: "media",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streetcode_art_slide_template", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "streetcode_art_slide",
                schema: "media",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Index = table.Column<int>(type: "int", nullable: false),
                    StreetcodeId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streetcode_art_slide", x => x.Id);
                    table.ForeignKey(
                        name: "FK_streetcode_art_slide_streetcode_art_slide_template_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "media",
                        principalTable: "streetcode_art_slide_template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_streetcode_art_slide_streetcodes_StreetcodeId",
                        column: x => x.StreetcodeId,
                        principalSchema: "streetcode",
                        principalTable: "streetcodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtSlideItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtId = table.Column<int>(type: "int", nullable: false),
                    SlideId = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtSlideItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtSlideItem_arts_ArtId",
                        column: x => x.ArtId,
                        principalSchema: "media",
                        principalTable: "arts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtSlideItem_streetcode_art_slide_SlideId",
                        column: x => x.SlideId,
                        principalSchema: "media",
                        principalTable: "streetcode_art_slide",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "media",
                table: "streetcode_art_slide_template",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "OneToFourAndFiveToSix" },
                    { 1, "OneToTwoAndThreeToFourAndFiveToSix" },
                    { 2, "OneAndTwoAndThreeAndFourAndFiveAndSix" },
                    { 3, "OneToFour" },
                    { 4, "OneToTwo" },
                    { 5, "OneToTwoAndThreeToFour" },
                    { 6, "OneToFourAndFiveAndSix" },
                    { 7, "OneAndTwoAndThreeToFour" },
                    { 8, "OneAndTwoAndThreeAndFour" },
                    { 9, "OneToTwoAndThreeToFourAndFive" },
                    { 10, "OneAndTwoAndThreeToFourAndFiveToSix" },
                    { 11, "OneAndTwoAndThreeToFourAndFive" },
                    { 12, "OneAndTwoAndThreeToFourAndFiveAndSix" },
                    { 13, "OneAndTwoAndThreeAndFourAndFive" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtSlideItem_ArtId",
                table: "ArtSlideItem",
                column: "ArtId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtSlideItem_SlideId",
                table: "ArtSlideItem",
                column: "SlideId");

            migrationBuilder.CreateIndex(
                name: "IX_streetcode_art_slide_StreetcodeId",
                schema: "media",
                table: "streetcode_art_slide",
                column: "StreetcodeId");

            migrationBuilder.CreateIndex(
                name: "IX_streetcode_art_slide_TemplateId",
                schema: "media",
                table: "streetcode_art_slide",
                column: "TemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtSlideItem");

            migrationBuilder.DropTable(
                name: "streetcode_art_slide",
                schema: "media");

            migrationBuilder.DropTable(
                name: "streetcode_art_slide_template",
                schema: "media");
        }
    }
}
