using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseManagementData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    HasWaitlist = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    Total = table.Column<int>(nullable: false),
                    Open = table.Column<int>(nullable: false),
                    WaitlistLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseManagementData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HousingData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    HasWaitlist = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    EmergencySharedBedsTotal = table.Column<int>(nullable: false),
                    EmergencySharedBedsOpen = table.Column<int>(nullable: false),
                    EmergencySharedBedsWaitlistLength = table.Column<int>(nullable: false),
                    EmergencyPrivateBedsTotal = table.Column<int>(nullable: false),
                    EmergencyPrivateBedsOpen = table.Column<int>(nullable: false),
                    EmergencyPrivateBedsWaitlistLength = table.Column<int>(nullable: false),
                    LongTermSharedBedsTotal = table.Column<int>(nullable: false),
                    LongTermSharedBedsOpen = table.Column<int>(nullable: false),
                    LongTermSharedBedsWaitlistLength = table.Column<int>(nullable: false),
                    LongTermPrivateBedsTotal = table.Column<int>(nullable: false),
                    LongTermPrivateBedsOpen = table.Column<int>(nullable: false),
                    LongTermPrivateBedsWaitlistLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTrainingData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    HasWaitlist = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    Total = table.Column<int>(nullable: false),
                    Open = table.Column<int>(nullable: false),
                    WaitlistLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTrainingData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MentalHealthData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    HasWaitlist = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    InPatientTotal = table.Column<int>(nullable: false),
                    InPatientOpen = table.Column<int>(nullable: false),
                    InPatientWaitlistLength = table.Column<int>(nullable: false),
                    OutPatientTotal = table.Column<int>(nullable: false),
                    OutPatientOpen = table.Column<int>(nullable: false),
                    OutPatientWaitlistLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentalHealthData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsVerified = table.Column<bool>(nullable: false),
                    UpdateFrequency = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubstanceUseData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    HasWaitlist = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    DetoxTotal = table.Column<int>(nullable: false),
                    DetoxOpen = table.Column<int>(nullable: false),
                    DetoxWaitlistLength = table.Column<int>(nullable: false),
                    InPatientTotal = table.Column<int>(nullable: false),
                    InPatientOpen = table.Column<int>(nullable: false),
                    InPatientWaitlistLength = table.Column<int>(nullable: false),
                    OutPatientTotal = table.Column<int>(nullable: false),
                    OutPatientOpen = table.Column<int>(nullable: false),
                    OutPatientWaitlistLength = table.Column<int>(nullable: false),
                    GroupTotal = table.Column<int>(nullable: false),
                    GroupOpen = table.Column<int>(nullable: false),
                    GroupWaitlistLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstanceUseData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseManagementData");

            migrationBuilder.DropTable(
                name: "HousingData");

            migrationBuilder.DropTable(
                name: "JobTrainingData");

            migrationBuilder.DropTable(
                name: "MentalHealthData");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "SubstanceUseData");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
