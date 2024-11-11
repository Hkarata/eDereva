using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActualStartTime",
                table: "Sessions",
                newName: "InitiationTime");

            migrationBuilder.AddColumn<string>(
                name: "ContingencyExplanation",
                table: "Sessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContingencyExplanation",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "InitiationTime",
                table: "Sessions",
                newName: "ActualStartTime");
        }
    }
}
