using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock741.Migrations
{
    /// <inheritdoc />
    public partial class AddDatePretAndEdsAutomatiqueAffectation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EdsAutomatiqueId",
                table: "Affectations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Affectations_EdsAutomatiqueId",
                table: "Affectations",
                column: "EdsAutomatiqueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Affectations_Eds_EdsAutomatiqueId",
                table: "Affectations",
                column: "EdsAutomatiqueId",
                principalTable: "Eds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Affectations_Eds_EdsAutomatiqueId",
                table: "Affectations");

            migrationBuilder.DropIndex(
                name: "IX_Affectations_EdsAutomatiqueId",
                table: "Affectations");

            migrationBuilder.DropColumn(
                name: "EdsAutomatiqueId",
                table: "Affectations");
        }
    }
}
