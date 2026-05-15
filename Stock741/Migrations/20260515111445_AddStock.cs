using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asset = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumReception = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    StatutId = table.Column<int>(type: "int", nullable: true),
                    LieuId = table.Column<int>(type: "int", nullable: true),
                    Colis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModeleId = table.Column<int>(type: "int", nullable: false),
                    FournisseurId = table.Column<int>(type: "int", nullable: true),
                    NumSerie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Qte = table.Column<int>(type: "int", nullable: false),
                    Garantie = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemeId = table.Column<int>(type: "int", nullable: true),
                    NumSim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Imei1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Imei2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stocks_Lieux_LieuId",
                        column: x => x.LieuId,
                        principalTable: "Lieux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stocks_Modeles_ModeleId",
                        column: x => x.ModeleId,
                        principalTable: "Modeles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stocks_Statuts_StatutId",
                        column: x => x.StatutId,
                        principalTable: "Statuts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stocks_Systemes_SystemeId",
                        column: x => x.SystemeId,
                        principalTable: "Systemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Asset",
                table: "Stocks",
                column: "Asset",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_FournisseurId",
                table: "Stocks",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_LieuId",
                table: "Stocks",
                column: "LieuId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ModeleId",
                table: "Stocks",
                column: "ModeleId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_StatutId",
                table: "Stocks",
                column: "StatutId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_SystemeId",
                table: "Stocks",
                column: "SystemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
