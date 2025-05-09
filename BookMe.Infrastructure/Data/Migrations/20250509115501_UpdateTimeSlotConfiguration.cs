using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMe.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTimeSlotConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Users_CreatedBy",
                table: "TimeSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Users_UpdatedBy",
                table: "TimeSlots");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Users_CreatedBy",
                table: "TimeSlots",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Users_UpdatedBy",
                table: "TimeSlots",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
    }
}
