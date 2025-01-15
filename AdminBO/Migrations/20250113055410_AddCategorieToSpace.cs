using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBO.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorieToSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "SpaceActivities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "CategoryActivities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryActivities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceActivities_CategoryId",
                table: "SpaceActivities",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceActivities_CategoryActivities_CategoryId",
                table: "SpaceActivities",
                column: "CategoryId",
                principalTable: "CategoryActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaceActivities_CategoryActivities_CategoryId",
                table: "SpaceActivities");

            migrationBuilder.DropTable(
                name: "CategoryActivities");

            migrationBuilder.DropIndex(
                name: "IX_SpaceActivities_CategoryId",
                table: "SpaceActivities");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "SpaceActivities");
        }
    }
}
