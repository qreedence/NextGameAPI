using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameSuggestionmodeltocontainsomeinfoaboutgame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameCoverUrl",
                table: "GameSuggestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GameName",
                table: "GameSuggestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameCoverUrl",
                table: "GameSuggestions");

            migrationBuilder.DropColumn(
                name: "GameName",
                table: "GameSuggestions");
        }
    }
}
