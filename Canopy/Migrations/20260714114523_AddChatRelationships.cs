using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canopy.Migrations
{
    /// <inheritdoc />
    public partial class AddChatRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Group_Chat_Group",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_Chat_Group",
                table: "Chat");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Chat_Group",
                table: "Chat",
                column: "Chat_Group",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Group",
                table: "Chat",
                column: "Chat_Group",
                principalTable: "Group",
                principalColumn: "Group_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Group",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_Chat_Group",
                table: "Chat");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Chat_Group",
                table: "Chat",
                column: "Chat_Group");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Group_Chat_Group",
                table: "Chat",
                column: "Chat_Group",
                principalTable: "Group",
                principalColumn: "Group_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

