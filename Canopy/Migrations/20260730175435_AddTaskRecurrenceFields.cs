using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canopy.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskRecurrenceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Recurrence",
                table: "PlannedTask",
                newName: "Task_Recurrence");

            migrationBuilder.AddColumn<bool>(
                name: "Task_IsRecurrenceEnded",
                table: "PlannedTask",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Task_RecurrenceMonthDay",
                table: "PlannedTask",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Task_RecurrenceWeekday",
                table: "PlannedTask",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Task_IsRecurrenceEnded",
                table: "PlannedTask");

            migrationBuilder.DropColumn(
                name: "Task_RecurrenceMonthDay",
                table: "PlannedTask");

            migrationBuilder.DropColumn(
                name: "Task_RecurrenceWeekday",
                table: "PlannedTask");

            migrationBuilder.RenameColumn(
                name: "Task_Recurrence",
                table: "PlannedTask",
                newName: "Recurrence");
        }
    }
}
