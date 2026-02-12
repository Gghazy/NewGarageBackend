using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cashif.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMechIssueTypeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MechIssueTypeId",
                table: "MechIssues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MechIssues_MechIssueTypeId",
                table: "MechIssues",
                column: "MechIssueTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MechIssues_MechIssueTypes_MechIssueTypeId",
                table: "MechIssues",
                column: "MechIssueTypeId",
                principalTable: "MechIssueTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MechIssues_MechIssueTypes_MechIssueTypeId",
                table: "MechIssues");

            migrationBuilder.DropIndex(
                name: "IX_MechIssues_MechIssueTypeId",
                table: "MechIssues");

            migrationBuilder.DropColumn(
                name: "MechIssueTypeId",
                table: "MechIssues");
        }
    }
}
