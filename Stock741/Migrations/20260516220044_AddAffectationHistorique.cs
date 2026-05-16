using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddAffectationHistorique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Affectations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: true),
                    EdsId = table.Column<int>(type: "int", nullable: true),
                    OperateurId = table.Column<int>(type: "int", nullable: true),
                    ForfaitId = table.Column<int>(type: "int", nullable: true),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NomAppareil = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdresseIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MasqueIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasserelleIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomPC = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    EdsPC = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AncienPC = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    NumTelMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Motif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actif = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Affectations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Affectations_Eds_EdsId",
                        column: x => x.EdsId,
                        principalTable: "Eds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Affectations_Forfaits_ForfaitId",
                        column: x => x.ForfaitId,
                        principalTable: "Forfaits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Affectations_Operateurs_OperateurId",
                        column: x => x.OperateurId,
                        principalTable: "Operateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Affectations_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Affectations_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueMouvements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    AffectationId = table.Column<int>(type: "int", nullable: true),
                    TypeMouvement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AncienStatutId = table.Column<int>(type: "int", nullable: true),
                    AncienLieuId = table.Column<int>(type: "int", nullable: true),
                    AncienUtilisateurId = table.Column<int>(type: "int", nullable: true),
                    AncienEdsId = table.Column<int>(type: "int", nullable: true),
                    AncienNomPC = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    AncienNomAppareil = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AncienAdresseIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AncienMasqueIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnciennePasserelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectuePar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueMouvements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueMouvements_Affectations_AffectationId",
                        column: x => x.AffectationId,
                        principalTable: "Affectations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriqueMouvements_Eds_AncienEdsId",
                        column: x => x.AncienEdsId,
                        principalTable: "Eds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriqueMouvements_Lieux_AncienLieuId",
                        column: x => x.AncienLieuId,
                        principalTable: "Lieux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriqueMouvements_Statuts_AncienStatutId",
                        column: x => x.AncienStatutId,
                        principalTable: "Statuts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriqueMouvements_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriqueMouvements_Utilisateurs_AncienUtilisateurId",
                        column: x => x.AncienUtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Affectations_EdsId",
                table: "Affectations",
                column: "EdsId");

            migrationBuilder.CreateIndex(
                name: "IX_Affectations_ForfaitId",
                table: "Affectations",
                column: "ForfaitId");

            migrationBuilder.CreateIndex(
                name: "IX_Affectations_OperateurId",
                table: "Affectations",
                column: "OperateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Affectations_StockId",
                table: "Affectations",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Affectations_UtilisateurId",
                table: "Affectations",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_AffectationId",
                table: "HistoriqueMouvements",
                column: "AffectationId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_AncienEdsId",
                table: "HistoriqueMouvements",
                column: "AncienEdsId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_AncienLieuId",
                table: "HistoriqueMouvements",
                column: "AncienLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_AncienStatutId",
                table: "HistoriqueMouvements",
                column: "AncienStatutId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_AncienUtilisateurId",
                table: "HistoriqueMouvements",
                column: "AncienUtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMouvements_StockId",
                table: "HistoriqueMouvements",
                column: "StockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoriqueMouvements");

            migrationBuilder.DropTable(
                name: "Affectations");
        }
    }
}
