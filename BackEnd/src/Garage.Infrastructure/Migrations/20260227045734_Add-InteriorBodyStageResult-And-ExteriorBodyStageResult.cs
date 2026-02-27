using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInteriorBodyStageResultAndExteriorBodyStageResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExteriorBodyStageResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NoIssuesFound = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ExteriorBodyStageResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExteriorBodyStageResults_Examinations_ExaminationId",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteriorBodyStageResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NoIssuesFound = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_InteriorBodyStageResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteriorBodyStageResults_Examinations_ExaminationId",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExteriorBodyStageResultItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExteriorBodyPartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExteriorBodyIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExteriorBodyStageResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_ExteriorBodyStageResultItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExteriorBodyStageResultItems_ExteriorBodyStageResults_ExteriorBodyStageResultId",
                        column: x => x.ExteriorBodyStageResultId,
                        principalTable: "ExteriorBodyStageResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteriorBodyStageResultItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InteriorBodyPartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InteriorBodyIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InteriorBodyStageResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_InteriorBodyStageResultItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteriorBodyStageResultItems_InteriorBodyStageResults_InteriorBodyStageResultId",
                        column: x => x.InteriorBodyStageResultId,
                        principalTable: "InteriorBodyStageResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExteriorBodyStageResultItems_ExteriorBodyStageResultId",
                table: "ExteriorBodyStageResultItems",
                column: "ExteriorBodyStageResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ExteriorBodyStageResults_ExaminationId",
                table: "ExteriorBodyStageResults",
                column: "ExaminationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InteriorBodyStageResultItems_InteriorBodyStageResultId",
                table: "InteriorBodyStageResultItems",
                column: "InteriorBodyStageResultId");

            migrationBuilder.CreateIndex(
                name: "IX_InteriorBodyStageResults_ExaminationId",
                table: "InteriorBodyStageResults",
                column: "ExaminationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExteriorBodyStageResultItems");

            migrationBuilder.DropTable(
                name: "InteriorBodyStageResultItems");

            migrationBuilder.DropTable(
                name: "ExteriorBodyStageResults");

            migrationBuilder.DropTable(
                name: "InteriorBodyStageResults");
        }
    }
}
