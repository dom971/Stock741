using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AjoutActifMateriel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Actif",
                table: "Materiels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actif",
                table: "Materiels");
        }
    }
}
