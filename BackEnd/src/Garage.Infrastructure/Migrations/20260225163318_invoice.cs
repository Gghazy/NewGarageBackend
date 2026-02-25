using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class invoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExaminationPayments_Examinations_ExaminationId",
                table: "ExaminationPayments");

            migrationBuilder.DropIndex(
                name: "IX_ExaminationPayments_ExaminationId",
                table: "ExaminationPayments");

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
                name: "TotalAmount",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TotalCurrency",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TotalWithTaxAmount",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TotalWithTaxCurrency",
                table: "Examinations");

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClientEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BranchNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, defaultValue: 0.15m),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalWithTaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalWithTaxCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPriceCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalPriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPriceCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicePayments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_InvoiceId",
                table: "InvoicePayments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ExaminationId",
                table: "Invoices",
                column: "ExaminationId",
                unique: true,
                filter: "[ExaminationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "InvoicePayments");

            migrationBuilder.DropTable(
                name: "Invoices");

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
                name: "TotalAmount",
                table: "Examinations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TotalCurrency",
                table: "Examinations",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationPayments_ExaminationId",
                table: "ExaminationPayments",
                column: "ExaminationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminationPayments_Examinations_ExaminationId",
                table: "ExaminationPayments",
                column: "ExaminationId",
                principalTable: "Examinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
