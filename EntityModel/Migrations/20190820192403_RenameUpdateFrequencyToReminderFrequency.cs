using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class RenameUpdateFrequencyToReminderFrequency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateFrequency",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "ReminderFrequency",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderFrequency",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UpdateFrequency",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }
    }
}
