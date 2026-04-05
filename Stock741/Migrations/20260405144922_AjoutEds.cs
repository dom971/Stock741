using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AjoutEds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cnx = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adr1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adr2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adr3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adr4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorLundi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorMardi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorMercredi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorJeudi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorVendredi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorSamedi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Geolocalisation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MailContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actif = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eds", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eds_Cnx",
                table: "Eds",
                column: "Cnx",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Eds");
        }
    }
}
