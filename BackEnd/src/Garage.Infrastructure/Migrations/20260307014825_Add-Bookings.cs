using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BranchNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManufacturerNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ManufacturerNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CarMarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarMarkNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CarMarkNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Transmission = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExaminationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExaminationTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConvertedExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
