using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserLicenseUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "LicenseClassUser",
                columns: table => new
                {
                    LicensesClassesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersNin = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseClassUser", x => new { x.LicensesClassesId, x.UsersNin });
                    table.ForeignKey(
                        name: "FK_LicenseClassUser_LicenseClasses_LicensesClassesId",
                        column: x => x.LicensesClassesId,
                        principalTable: "LicenseClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseClassUser_Users_UsersNin",
                        column: x => x.UsersNin,
                        principalTable: "Users",
                        principalColumn: "Nin",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LicenseClassUser_UsersNin",
                table: "LicenseClassUser",
                column: "UsersNin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenseClassUser");

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
    }
}
