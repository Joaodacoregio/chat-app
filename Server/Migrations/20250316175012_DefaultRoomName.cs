using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chatApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class DefaultRoomName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 1,
                column: "Name",
                value: "All");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 1,
                column: "Name",
                value: "");
        }
    }
}
