using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateinvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPriceAmount",
                table: "ExaminationItems");

            migrationBuilder.DropColumn(
                name: "DefaultPriceCurrency",
                table: "ExaminationItems");

            migrationBuilder.DropColumn(
                name: "PriceAmount",
                table: "ExaminationItems");

            migrationBuilder.DropColumn(
                name: "PriceCurrency",
                table: "ExaminationItems");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ExaminationItems",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ExaminationItems");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPriceAmount",
                table: "ExaminationItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DefaultPriceCurrency",
                table: "ExaminationItems",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAmount",
                table: "ExaminationItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceCurrency",
                table: "ExaminationItems",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }
    }
}
