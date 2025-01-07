using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LicenseClassMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserNin",
                table: "LicenseClasses",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenseClasses_UserNin",
                table: "LicenseClasses",
                column: "UserNin");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseClasses_Users_UserNin",
                table: "LicenseClasses",
                column: "UserNin",
                principalTable: "Users",
                principalColumn: "Nin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseClasses_Users_UserNin",
                table: "LicenseClasses");

            migrationBuilder.DropIndex(
                name: "IX_LicenseClasses_UserNin",
                table: "LicenseClasses");

            migrationBuilder.DropColumn(
                name: "UserNin",
                table: "LicenseClasses");
        }
    }
}
