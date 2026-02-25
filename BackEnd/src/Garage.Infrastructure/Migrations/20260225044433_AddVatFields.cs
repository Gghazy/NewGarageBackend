using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVatFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Examinations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TaxCurrency",
                table: "Examinations",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "Examinations",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0.15m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWithTaxAmount",
                table: "Examinations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TotalWithTaxCurrency",
                table: "Examinations",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TaxCurrency",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TotalWithTaxAmount",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TotalWithTaxCurrency",
                table: "Examinations");
        }
    }
}
