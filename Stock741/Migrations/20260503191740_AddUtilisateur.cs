using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddUtilisateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Societe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdUtilisateur = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomComplet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TelephoneMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TelephoneProfessionnel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Emplacement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Departement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bureau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodePostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodePays = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    IdWindows = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Vip = table.Column<bool>(type: "bit", nullable: false),
                    Actif = table.Column<bool>(type: "bit", nullable: false),
                    Manager = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FuseauHoraire = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreePar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MisAJourPar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateMiseAJour = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_IdWindows",
                table: "Utilisateurs",
                column: "IdWindows",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
