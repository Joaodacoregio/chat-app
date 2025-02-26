using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chatApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserInitialCreateTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Imagem",
                table: "Users",
                newName: "Img");

            migrationBuilder.RenameColumn(
                name: "Apelido",
                table: "Users",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "Apelido");

            migrationBuilder.RenameColumn(
                name: "Img",
                table: "Users",
                newName: "Imagem");
        }
    }
}
