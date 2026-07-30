using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canopy.Migrations
{
    /// <inheritdoc />
    public partial class fixOnDeleteForTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedTask_Group",
                table: "PlannedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedTask_Project",
                table: "PlannedTask");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedTask_Group",
                table: "PlannedTask",
                column: "Task_Group",
                principalTable: "Group",
                principalColumn: "Group_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedTask_Project",
                table: "PlannedTask",
                column: "Task_Project",
                principalTable: "Project",
                principalColumn: "Project_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedTask_Group",
                table: "PlannedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedTask_Project",
                table: "PlannedTask");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedTask_Group",
                table: "PlannedTask",
                column: "Task_Group",
                principalTable: "Group",
                principalColumn: "Group_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedTask_Project",
                table: "PlannedTask",
                column: "Task_Project",
                principalTable: "Project",
                principalColumn: "Project_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
