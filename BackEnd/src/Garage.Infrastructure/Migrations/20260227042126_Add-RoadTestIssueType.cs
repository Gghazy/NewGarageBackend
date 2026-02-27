using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoadTestIssueType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "RoadTestIssues",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "RoadTestIssueTypeId",
                table: "RoadTestIssues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "RoadTestIssueTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadTestIssueTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadTestIssues_RoadTestIssueTypeId",
                table: "RoadTestIssues",
                column: "RoadTestIssueTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadTestIssues_RoadTestIssueTypes_RoadTestIssueTypeId",
                table: "RoadTestIssues",
                column: "RoadTestIssueTypeId",
                principalTable: "RoadTestIssueTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadTestIssues_RoadTestIssueTypes_RoadTestIssueTypeId",
                table: "RoadTestIssues");

            migrationBuilder.DropTable(
                name: "RoadTestIssueTypes");

            migrationBuilder.DropIndex(
                name: "IX_RoadTestIssues_RoadTestIssueTypeId",
                table: "RoadTestIssues");

            migrationBuilder.DropColumn(
                name: "RoadTestIssueTypeId",
                table: "RoadTestIssues");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "RoadTestIssues",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
