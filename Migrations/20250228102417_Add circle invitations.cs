using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addcircleinvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CircleInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ToId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircleInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CircleInvitations_AspNetUsers_FromId",
                        column: x => x.FromId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircleInvitations_AspNetUsers_ToId",
                        column: x => x.ToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircleInvitations_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CircleInvitations_CircleId",
                table: "CircleInvitations",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleInvitations_FromId",
                table: "CircleInvitations",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleInvitations_ToId",
                table: "CircleInvitations",
                column: "ToId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CircleInvitations");
        }
    }
}
