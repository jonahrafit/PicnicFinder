using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBO.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationsForSpaceActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaceActivities_CategoryActivities_CategoryId",
                table: "SpaceActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceActivities_Spaces_SpaceId",
                table: "SpaceActivities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SpaceActivities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_Latitude_Longitude",
                table: "Spaces",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceActivities_Name",
                table: "SpaceActivities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryActivities_Name",
                table: "CategoryActivities",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceActivities_CategoryActivities_CategoryId",
                table: "SpaceActivities",
                column: "CategoryId",
                principalTable: "CategoryActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceActivities_Spaces_SpaceId",
                table: "SpaceActivities",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaceActivities_CategoryActivities_CategoryId",
                table: "SpaceActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceActivities_Spaces_SpaceId",
                table: "SpaceActivities");

            migrationBuilder.DropIndex(
                name: "IX_Spaces_Latitude_Longitude",
                table: "Spaces");

            migrationBuilder.DropIndex(
                name: "IX_SpaceActivities_Name",
                table: "SpaceActivities");

            migrationBuilder.DropIndex(
                name: "IX_CategoryActivities_Name",
                table: "CategoryActivities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SpaceActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceActivities_CategoryActivities_CategoryId",
                table: "SpaceActivities",
                column: "CategoryId",
                principalTable: "CategoryActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceActivities_Spaces_SpaceId",
                table: "SpaceActivities",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id");
        }
    }
}
