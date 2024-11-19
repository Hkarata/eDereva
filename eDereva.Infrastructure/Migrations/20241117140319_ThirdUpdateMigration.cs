using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThirdUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Contingencies_ContingencyId",
                table: "Sessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContingencyId",
                table: "Sessions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Contingencies_ContingencyId",
                table: "Sessions",
                column: "ContingencyId",
                principalTable: "Contingencies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Contingencies_ContingencyId",
                table: "Sessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContingencyId",
                table: "Sessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Contingencies_ContingencyId",
                table: "Sessions",
                column: "ContingencyId",
                principalTable: "Contingencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
