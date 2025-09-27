using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DroneService.Data.Migrations
{
    /// <inheritdoc />
    public partial class LpisId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArcGisId",
                table: "Field");

            migrationBuilder.AddColumn<Guid>(
                name: "ArcGisId",
                table: "AppUser",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArcGisId",
                table: "AppUser");

            migrationBuilder.AddColumn<int>(
                name: "ArcGisId",
                table: "Field",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
