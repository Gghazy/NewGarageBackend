using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatemechIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MechIssues_MechIssueTypes_MechIssueTypeId",
                table: "MechIssues");

            migrationBuilder.DropTable(
                name: "MechIssueTypes");

            migrationBuilder.DropIndex(
                name: "IX_MechIssues_MechIssueTypeId",
                table: "MechIssues");

            migrationBuilder.DropColumn(
                name: "MechIssueTypeId",
                table: "MechIssues");

            migrationBuilder.RenameColumn(
                name: "MechIssueTypeId",
                table: "MechanicalStageResultItems",
                newName: "MechPartTypeId");

            migrationBuilder.RenameColumn(
                name: "MechIssueId",
                table: "MechanicalStageResultItems",
                newName: "MechPartId");

            migrationBuilder.CreateTable(
                name: "MechanicalStageResultIssueItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MechPartTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MechIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MechanicalStageResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_MechanicalStageResultIssueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MechanicalStageResultIssueItems_MechanicalStageResults_MechanicalStageResultId",
                        column: x => x.MechanicalStageResultId,
                        principalTable: "MechanicalStageResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MechPartTypes",
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
                    table.PrimaryKey("PK_MechPartTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MechParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MechPartTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_MechParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MechParts_MechPartTypes_MechPartTypeId",
                        column: x => x.MechPartTypeId,
                        principalTable: "MechPartTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MechanicalStageResultIssueItems_MechanicalStageResultId",
                table: "MechanicalStageResultIssueItems",
                column: "MechanicalStageResultId");

            migrationBuilder.CreateIndex(
                name: "IX_MechParts_MechPartTypeId",
                table: "MechParts",
                column: "MechPartTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MechanicalStageResultIssueItems");

            migrationBuilder.DropTable(
                name: "MechParts");

            migrationBuilder.DropTable(
                name: "MechPartTypes");

            migrationBuilder.RenameColumn(
                name: "MechPartTypeId",
                table: "MechanicalStageResultItems",
                newName: "MechIssueTypeId");

            migrationBuilder.RenameColumn(
                name: "MechPartId",
                table: "MechanicalStageResultItems",
                newName: "MechIssueId");

            migrationBuilder.AddColumn<Guid>(
                name: "MechIssueTypeId",
                table: "MechIssues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "MechIssueTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MechIssueTypes", x => x.Id);
                });

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
    }
}
