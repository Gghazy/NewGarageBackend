using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateMechanicalStageResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MechPartTypeId",
                table: "MechanicalStageResultIssueItems",
                newName: "MechPartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MechPartId",
                table: "MechanicalStageResultIssueItems",
                newName: "MechPartTypeId");
        }
    }
}
