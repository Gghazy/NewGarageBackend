using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Examinations",
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
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleManufacturerNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VehicleManufacturerNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VehicleCarMarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleCarMarkNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VehicleCarMarkNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VehicleYear = table.Column<int>(type: "int", nullable: true),
                    VehicleColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleVin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleHasPlate = table.Column<bool>(type: "bit", nullable: false),
                    VehiclePlateLetters = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    VehiclePlateNumbers = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    VehicleMileage = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    VehicleMileageUnit = table.Column<int>(type: "int", nullable: false),
                    VehicleTransmission = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HasWarranty = table.Column<bool>(type: "bit", nullable: false),
                    HasPhotos = table.Column<bool>(type: "bit", nullable: false),
                    MarketerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
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
                    table.PrimaryKey("PK_Examinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManufacturerNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ManufacturerNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CarMarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarMarkNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CarMarkNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Vin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasPlate = table.Column<bool>(type: "bit", nullable: false),
                    PlateLetters = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PlateNumbers = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Mileage = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MileageUnit = table.Column<int>(type: "int", nullable: false),
                    Transmission = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExaminationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ServiceNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DefaultPriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultPriceCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ExaminationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminationItems_Examinations_ExaminationId",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationItems_ExaminationId",
                table: "ExaminationItems",
                column: "ExaminationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExaminationItems");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Examinations");
        }
    }
}
