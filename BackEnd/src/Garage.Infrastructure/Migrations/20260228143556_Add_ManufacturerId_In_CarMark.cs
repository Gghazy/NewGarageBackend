using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_ManufacturerId_In_CarMark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManufacturerId",
                table: "CarMarkes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CarMarkes_ManufacturerId",
                table: "CarMarkes",
                column: "ManufacturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarMarkes_Manufacturers_ManufacturerId",
                table: "CarMarkes",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarMarkes_Manufacturers_ManufacturerId",
                table: "CarMarkes");

            migrationBuilder.DropIndex(
                name: "IX_CarMarkes_ManufacturerId",
                table: "CarMarkes");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "CarMarkes");
        }
    }
}
