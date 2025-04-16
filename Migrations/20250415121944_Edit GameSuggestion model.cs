using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class EditGameSuggestionmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSuggestions_Circles_CircleId",
                table: "GameSuggestions");

            migrationBuilder.AlterColumn<Guid>(
                name: "CircleId",
                table: "GameSuggestions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSuggestions_Circles_CircleId",
                table: "GameSuggestions",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSuggestions_Circles_CircleId",
                table: "GameSuggestions");

            migrationBuilder.AlterColumn<Guid>(
                name: "CircleId",
                table: "GameSuggestions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSuggestions_Circles_CircleId",
                table: "GameSuggestions",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "Id");
        }
    }
}
