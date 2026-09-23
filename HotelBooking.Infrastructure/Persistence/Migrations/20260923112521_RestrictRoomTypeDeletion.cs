using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestrictRoomTypeDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_room_types_RoomTypeId",
                table: "rooms");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_room_types_RoomTypeId",
                table: "rooms",
                column: "RoomTypeId",
                principalTable: "room_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_room_types_RoomTypeId",
                table: "rooms");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_room_types_RoomTypeId",
                table: "rooms",
                column: "RoomTypeId",
                principalTable: "room_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
