using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMe.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableFieldsToTimeSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "TimeSlots",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TimeSlots",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "TimeSlots",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "TimeSlots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_CreatedBy",
                table: "TimeSlots",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_UpdatedBy",
                table: "TimeSlots",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Users_CreatedBy",
                table: "TimeSlots",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Users_UpdatedBy",
                table: "TimeSlots",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Users_CreatedBy",
                table: "TimeSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Users_UpdatedBy",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_CreatedBy",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_UpdatedBy",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TimeSlots");
        }
    }
}
