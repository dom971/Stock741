using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddDatesAffectationHistorique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDebutAffectation",
                table: "HistoriqueMouvements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinAffectation",
                table: "HistoriqueMouvements",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDebutAffectation",
                table: "HistoriqueMouvements");

            migrationBuilder.DropColumn(
                name: "DateFinAffectation",
                table: "HistoriqueMouvements");
        }
    }
}
