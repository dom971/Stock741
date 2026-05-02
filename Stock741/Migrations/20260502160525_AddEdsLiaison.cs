using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddEdsLiaison : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EdsLiaisons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cible = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EdsId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdsLiaisons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdsLiaisons_Eds_EdsId",
                        column: x => x.EdsId,
                        principalTable: "Eds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EdsLiaisons_EdsId",
                table: "EdsLiaisons",
                column: "EdsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EdsLiaisons");
        }
    }
}
