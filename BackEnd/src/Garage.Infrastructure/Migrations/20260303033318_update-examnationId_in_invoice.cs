using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateexamnationId_in_invoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_ExaminationId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AdjustmentAmount",
                table: "InvoiceItems");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ExaminationId",
                table: "Invoices",
                column: "ExaminationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_ExaminationId",
                table: "Invoices");

            migrationBuilder.AddColumn<decimal>(
                name: "AdjustmentAmount",
                table: "InvoiceItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ExaminationId",
                table: "Invoices",
                column: "ExaminationId",
                unique: true,
                filter: "[ExaminationId] IS NOT NULL");
        }
    }
}
