using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMe.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleBookingsPerTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                unique: true);
        }
    }
}
