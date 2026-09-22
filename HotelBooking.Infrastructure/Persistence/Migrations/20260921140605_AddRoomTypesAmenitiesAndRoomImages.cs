using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HotelBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTypesAmenitiesAndRoomImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "rooms");

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeId",
                table: "rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "amenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "room_images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_room_images_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "hotel_amenities",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    AmenityId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_amenities", x => new { x.HotelId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_hotel_amenities_amenities_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hotel_amenities_hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rooms_RoomTypeId",
                table: "rooms",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_amenities_Name",
                table: "amenities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hotel_amenities_AmenityId",
                table: "hotel_amenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_room_images_RoomId",
                table: "room_images",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_room_types_RoomTypeId",
                table: "rooms",
                column: "RoomTypeId",
                principalTable: "room_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_room_types_RoomTypeId",
                table: "rooms");

            migrationBuilder.DropTable(
                name: "hotel_amenities");

            migrationBuilder.DropTable(
                name: "room_images");

            migrationBuilder.DropTable(
                name: "room_types");

            migrationBuilder.DropTable(
                name: "amenities");

            migrationBuilder.DropIndex(
                name: "IX_rooms_RoomTypeId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "RoomTypeId",
                table: "rooms");

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "rooms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
