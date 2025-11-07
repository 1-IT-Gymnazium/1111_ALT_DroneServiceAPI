using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DroneService.Data.Migrations
{
    /// <inheritdoc />
    public partial class reservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Reservation");

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscription",
                table: "Reservation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Reservation",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationId",
                table: "Field",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Field_ReservationId",
                table: "Field",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Field_Reservation_ReservationId",
                table: "Field",
                column: "ReservationId",
                principalTable: "Reservation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Field_Reservation_ReservationId",
                table: "Field");

            migrationBuilder.DropIndex(
                name: "IX_Field_ReservationId",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "IsSubscription",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Field");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Reservation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
