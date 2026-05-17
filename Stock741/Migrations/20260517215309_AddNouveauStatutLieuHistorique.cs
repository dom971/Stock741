using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddNouveauStatutLieuHistorique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NouveauLieuId",
                table: "HistoriqueMouvements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NouveauStatutId",
                table: "HistoriqueMouvements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_NouveauLieuId",
                table: "HistoriqueMouvements",
                column: "NouveauLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_NouveauStatutId",
                table: "HistoriqueMouvements",
                column: "NouveauStatutId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueMouvements_Lieux_NouveauLieuId",
                table: "HistoriqueMouvements",
                column: "NouveauLieuId",
                principalTable: "Lieux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueMouvements_Statuts_NouveauStatutId",
                table: "HistoriqueMouvements",
                column: "NouveauStatutId",
                principalTable: "Statuts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueMouvements_Lieux_NouveauLieuId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueMouvements_Statuts_NouveauStatutId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropIndex(
                name: "IX_HistoriqueMouvements_NouveauLieuId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropIndex(
                name: "IX_HistoriqueMouvements_NouveauStatutId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropColumn(
                name: "NouveauLieuId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropColumn(
                name: "NouveauStatutId",
                table: "HistoriqueMouvements");
        }
    }
}
