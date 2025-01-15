using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBO.Migrations
{
    /// <inheritdoc />
    public partial class AddSpaceActivityLInk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaceActivities_Spaces_SpaceId",
                table: "SpaceActivities");

            migrationBuilder.DropIndex(
                name: "IX_SpaceActivities_SpaceId",
                table: "SpaceActivities");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "SpaceActivities");

            migrationBuilder.CreateTable(
                name: "SpaceActivityLinks",
                columns: table => new
                {
                    SpaceId = table.Column<long>(type: "bigint", nullable: false),
                    SpaceActivityId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceActivityLinks", x => new { x.SpaceId, x.SpaceActivityId });
                    table.ForeignKey(
                        name: "FK_SpaceActivityLinks_SpaceActivities_SpaceActivityId",
                        column: x => x.SpaceActivityId,
                        principalTable: "SpaceActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpaceActivityLinks_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceActivityLinks_SpaceActivityId",
                table: "SpaceActivityLinks",
                column: "SpaceActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpaceActivityLinks");

            migrationBuilder.AddColumn<long>(
                name: "SpaceId",
                table: "SpaceActivities",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpaceActivities_SpaceId",
                table: "SpaceActivities",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceActivities_Spaces_SpaceId",
                table: "SpaceActivities",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
