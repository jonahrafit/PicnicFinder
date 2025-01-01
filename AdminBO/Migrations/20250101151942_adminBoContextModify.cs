using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBO.Migrations
{
    /// <inheritdoc />
    public partial class adminBoContextModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Space_Users_OwnerId",
                table: "Space");

            migrationBuilder.AddForeignKey(
                name: "FK_Space_Users_OwnerId",
                table: "Space",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Space_Users_OwnerId",
                table: "Space");

            migrationBuilder.AddForeignKey(
                name: "FK_Space_Users_OwnerId",
                table: "Space",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
