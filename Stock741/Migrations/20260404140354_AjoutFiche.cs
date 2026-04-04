using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AjoutFiche : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fiches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fiches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fiches_Nom",
                table: "Fiches",
                column: "Nom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fiches");
        }
    }
}
