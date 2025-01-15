using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBO.Migrations
{
    /// <inheritdoc />
    public partial class AddActivitiesToSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Space_SpaceId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Space_SpaceId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Space_Users_OwnerId",
                table: "Space");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Space",
                table: "Space");

            migrationBuilder.RenameTable(
                name: "Space",
                newName: "Spaces");

            migrationBuilder.RenameIndex(
                name: "IX_Space_OwnerId",
                table: "Spaces",
                newName: "IX_Spaces_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spaces",
                table: "Spaces",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SpaceActivities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpaceId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpaceActivities_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceActivities_SpaceId",
                table: "SpaceActivities",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Spaces_SpaceId",
                table: "Favorites",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Spaces_SpaceId",
                table: "Reservations",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Spaces_Users_OwnerId",
                table: "Spaces",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Spaces_SpaceId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Spaces_SpaceId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Spaces_Users_OwnerId",
                table: "Spaces");

            migrationBuilder.DropTable(
                name: "SpaceActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spaces",
                table: "Spaces");

            migrationBuilder.RenameTable(
                name: "Spaces",
                newName: "Space");

            migrationBuilder.RenameIndex(
                name: "IX_Spaces_OwnerId",
                table: "Space",
                newName: "IX_Space_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Space",
                table: "Space",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Space_SpaceId",
                table: "Favorites",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Space_SpaceId",
                table: "Reservations",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Space_Users_OwnerId",
                table: "Space",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
