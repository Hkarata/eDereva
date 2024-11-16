using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondUpdateMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Users_UsersNIN",
                table: "RoleUser");

            migrationBuilder.RenameColumn(
                name: "NIN",
                table: "Users",
                newName: "Nin");

            migrationBuilder.RenameColumn(
                name: "UsersNIN",
                table: "RoleUser",
                newName: "UsersNin");

            migrationBuilder.RenameIndex(
                name: "IX_RoleUser_UsersNIN",
                table: "RoleUser",
                newName: "IX_RoleUser_UsersNin");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Users_UsersNin",
                table: "RoleUser",
                column: "UsersNin",
                principalTable: "Users",
                principalColumn: "Nin",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Users_UsersNin",
                table: "RoleUser");

            migrationBuilder.RenameColumn(
                name: "Nin",
                table: "Users",
                newName: "NIN");

            migrationBuilder.RenameColumn(
                name: "UsersNin",
                table: "RoleUser",
                newName: "UsersNIN");

            migrationBuilder.RenameIndex(
                name: "IX_RoleUser_UsersNin",
                table: "RoleUser",
                newName: "IX_RoleUser_UsersNIN");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Users_UsersNIN",
                table: "RoleUser",
                column: "UsersNIN",
                principalTable: "Users",
                principalColumn: "NIN",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
