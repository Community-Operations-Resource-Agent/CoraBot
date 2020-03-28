using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class ResourceUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "HasWaitlist",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "Max",
                table: "Resources",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "IsWaitlistOpen",
                table: "Resources",
                newName: "HasQuantity");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Feedback",
                newName: "CreatedById");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Resources",
                newName: "Max");

            migrationBuilder.RenameColumn(
                name: "HasQuantity",
                table: "Resources",
                newName: "IsWaitlistOpen");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Feedback",
                newName: "SenderId");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Available",
                table: "Resources",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasWaitlist",
                table: "Resources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Resources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsVerified = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });
        }
    }
}
