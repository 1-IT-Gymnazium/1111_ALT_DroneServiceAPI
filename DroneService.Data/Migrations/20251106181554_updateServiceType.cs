using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace DroneService.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateServiceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Reservation");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "ServiceType",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Instant>(
                name: "CreatedAt",
                table: "ServiceType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ServiceType",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Instant>(
                name: "DeletedAt",
                table: "ServiceType",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ServiceType",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "ModifiedAt",
                table: "ServiceType",
                type: "timestamp with time zone",
                maxLength: 200,
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ServiceType",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceType_AuthorId",
                table: "ServiceType",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceType_AppUser_AuthorId",
                table: "ServiceType",
                column: "AuthorId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceType_AppUser_AuthorId",
                table: "ServiceType");

            migrationBuilder.DropIndex(
                name: "IX_ServiceType_AuthorId",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ServiceType");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Reservation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
