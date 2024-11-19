using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VenueExemptions",
                columns: table => new
                {
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExemptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HasBeenApproved = table.Column<bool>(type: "bit", nullable: false),
                    HasBeenExempted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueExemptions", x => x.VenueId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VenueExemptions");
        }
    }
}
