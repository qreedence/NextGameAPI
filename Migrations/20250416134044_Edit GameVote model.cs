using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class EditGameVotemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "GameVotes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_GameVotes_UserId",
                table: "GameVotes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameVotes_AspNetUsers_UserId",
                table: "GameVotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameVotes_AspNetUsers_UserId",
                table: "GameVotes");

            migrationBuilder.DropIndex(
                name: "IX_GameVotes_UserId",
                table: "GameVotes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "GameVotes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
