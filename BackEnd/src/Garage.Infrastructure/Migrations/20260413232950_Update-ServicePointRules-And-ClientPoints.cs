using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServicePointRulesAndClientPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServicePointRules_ServiceId",
                table: "ServicePointRules");

            migrationBuilder.DropIndex(
                name: "IX_ServicePointRules_ServiceId_StartDate_EndDate",
                table: "ServicePointRules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServicePointRule_DateRange",
                table: "ServicePointRules");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ServicePointRules");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServicePointRules");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ServicePointRules");

            migrationBuilder.AddColumn<decimal>(
                name: "FromAmount",
                table: "ServicePointRules",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ToAmount",
                table: "ServicePointRules",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ServicePointRules_FromAmount_ToAmount",
                table: "ServicePointRules",
                columns: new[] { "FromAmount", "ToAmount" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServicePointRule_AmountRange",
                table: "ServicePointRules",
                sql: "[FromAmount] < [ToAmount]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServicePointRules_FromAmount_ToAmount",
                table: "ServicePointRules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServicePointRule_AmountRange",
                table: "ServicePointRules");

            migrationBuilder.DropColumn(
                name: "FromAmount",
                table: "ServicePointRules");

            migrationBuilder.DropColumn(
                name: "ToAmount",
                table: "ServicePointRules");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ServicePointRules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "ServicePointRules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ServicePointRules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ServicePointRules_ServiceId",
                table: "ServicePointRules",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePointRules_ServiceId_StartDate_EndDate",
                table: "ServicePointRules",
                columns: new[] { "ServiceId", "StartDate", "EndDate" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServicePointRule_DateRange",
                table: "ServicePointRules",
                sql: "[StartDate] <= [EndDate]");
        }
    }
}
