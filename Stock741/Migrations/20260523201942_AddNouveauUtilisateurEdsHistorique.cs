using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddNouveauUtilisateurEdsHistorique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NouveauEdsId",
                table: "HistoriqueMouvements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NouveauUtilisateurId",
                table: "HistoriqueMouvements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_NouveauEdsId",
                table: "HistoriqueMouvements",
                column: "NouveauEdsId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_NouveauUtilisateurId",
                table: "HistoriqueMouvements",
                column: "NouveauUtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueMouvements_Eds_NouveauEdsId",
                table: "HistoriqueMouvements",
                column: "NouveauEdsId",
                principalTable: "Eds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueMouvements_Utilisateurs_NouveauUtilisateurId",
                table: "HistoriqueMouvements",
                column: "NouveauUtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueMouvements_Eds_NouveauEdsId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueMouvements_Utilisateurs_NouveauUtilisateurId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropIndex(
                name: "IX_HistoriqueMouvements_NouveauEdsId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropIndex(
                name: "IX_HistoriqueMouvements_NouveauUtilisateurId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropColumn(
                name: "NouveauEdsId",
                table: "HistoriqueMouvements");

            migrationBuilder.DropColumn(
                name: "NouveauUtilisateurId",
                table: "HistoriqueMouvements");
        }
    }
}
