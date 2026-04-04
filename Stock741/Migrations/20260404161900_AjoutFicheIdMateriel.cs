using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AjoutFicheIdMateriel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FicheId",
                table: "Materiels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_FicheId",
                table: "Materiels",
                column: "FicheId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiels_Fiches_FicheId",
                table: "Materiels",
                column: "FicheId",
                principalTable: "Fiches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiels_Fiches_FicheId",
                table: "Materiels");

            migrationBuilder.DropIndex(
                name: "IX_Materiels_FicheId",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "FicheId",
                table: "Materiels");
        }
    }
}
