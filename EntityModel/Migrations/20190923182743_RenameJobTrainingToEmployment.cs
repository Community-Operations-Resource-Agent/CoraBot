using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class RenameJobTrainingToEmployment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobTrainingData");

            migrationBuilder.CreateTable(
                name: "EmploymentData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    IsComplete = table.Column<bool>(nullable: false),
                    JobReadinessTrainingTotal = table.Column<int>(nullable: false),
                    JobReadinessTrainingOpen = table.Column<int>(nullable: false),
                    JobReadinessTrainingHasWaitlist = table.Column<bool>(nullable: false),
                    JobReadinessTrainingWaitlistIsOpen = table.Column<bool>(nullable: false),
                    PaidInternshipTotal = table.Column<int>(nullable: false),
                    PaidInternshipOpen = table.Column<int>(nullable: false),
                    PaidInternshipHasWaitlist = table.Column<bool>(nullable: false),
                    PaidInternshipWaitlistIsOpen = table.Column<bool>(nullable: false),
                    VocationalTrainingTotal = table.Column<int>(nullable: false),
                    VocationalTrainingOpen = table.Column<int>(nullable: false),
                    VocationalTrainingHasWaitlist = table.Column<bool>(nullable: false),
                    VocationalTrainingWaitlistIsOpen = table.Column<bool>(nullable: false),
                    EmploymentPlacementTotal = table.Column<int>(nullable: false),
                    EmploymentPlacementOpen = table.Column<int>(nullable: false),
                    EmploymentPlacementHasWaitlist = table.Column<bool>(nullable: false),
                    EmploymentPlacementWaitlistIsOpen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmploymentData");

            migrationBuilder.CreateTable(
                name: "JobTrainingData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    HasWaitlist = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Open = table.Column<int>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    Total = table.Column<int>(nullable: false),
                    WaitlistIsOpen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTrainingData", x => x.Id);
                });
        }
    }
}
