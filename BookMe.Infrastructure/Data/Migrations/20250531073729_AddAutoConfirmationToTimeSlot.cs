using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMe.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoConfirmationToTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowAutoConfirmation",
                table: "TimeSlots",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAutoConfirmation",
                table: "TimeSlots");
        }
    }
}
