using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInteriorDecorStageResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "InteriorDecorStageResultItems");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "InteriorDecorStageResultItems");

            migrationBuilder.AddColumn<bool>(
                name: "NoIssuesFound",
                table: "InteriorDecorStageResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "InsideAndDecorPartId",
                table: "InteriorDecorStageResultItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InsideAndDecorPartIssueId",
                table: "InteriorDecorStageResultItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoIssuesFound",
                table: "InteriorDecorStageResults");

            migrationBuilder.DropColumn(
                name: "InsideAndDecorPartId",
                table: "InteriorDecorStageResultItems");

            migrationBuilder.DropColumn(
                name: "InsideAndDecorPartIssueId",
                table: "InteriorDecorStageResultItems");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "InteriorDecorStageResultItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "InteriorDecorStageResultItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
