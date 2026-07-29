using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canopy.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskOccurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Recurrence",
                table: "PlannedTask",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TaskOccurrence",
                columns: table => new
                {
                    TaskOccurrence_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskOccurrence_Task = table.Column<int>(type: "int", nullable: false),
                    TaskOccurrence_OccurrenceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskOccurrence_IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TaskOccurrence_CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskOccurrence", x => x.TaskOccurrence_Id);
                    table.ForeignKey(
                        name: "FK_PlannedTaskOccurrence_Task",
                        column: x => x.TaskOccurrence_Task,
                        principalTable: "PlannedTask",
                        principalColumn: "Task_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskOccurrence_TaskOccurrence_Task_TaskOccurrence_OccurrenceDate",
                table: "TaskOccurrence",
                columns: new[] { "TaskOccurrence_Task", "TaskOccurrence_OccurrenceDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskOccurrence");

            migrationBuilder.DropColumn(
                name: "Recurrence",
                table: "PlannedTask");
        }
    }
}
