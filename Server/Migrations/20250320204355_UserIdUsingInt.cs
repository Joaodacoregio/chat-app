using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chatApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserIdUsingInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId1",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId1",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId1",
                table: "Messages",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId1",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId1",
                table: "Messages",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId1",
                table: "Messages",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
