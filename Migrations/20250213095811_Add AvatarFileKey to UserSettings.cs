using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarFileKeytoUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarFileKey",
                table: "UserSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarFileKey",
                table: "UserSettings");
        }
    }
}
