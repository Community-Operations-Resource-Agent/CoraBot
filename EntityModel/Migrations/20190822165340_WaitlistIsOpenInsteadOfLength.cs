using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class WaitlistIsOpenInsteadOfLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetoxWaitlistLength",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "GroupWaitlistLength",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "InPatientWaitlistLength",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "OutPatientWaitlistLength",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "InPatientWaitlistLength",
                table: "MentalHealthData");

            migrationBuilder.DropColumn(
                name: "OutPatientWaitlistLength",
                table: "MentalHealthData");

            migrationBuilder.DropColumn(
                name: "WaitlistLength",
                table: "JobTrainingData");

            migrationBuilder.DropColumn(
                name: "EmergencyPrivateBedsWaitlistLength",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "EmergencySharedBedsWaitlistLength",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "LongTermPrivateBedsWaitlistLength",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "LongTermSharedBedsWaitlistLength",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "WaitlistLength",
                table: "CaseManagementData");

            migrationBuilder.AddColumn<bool>(
                name: "DetoxWaitlistIsOpen",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GroupWaitlistIsOpen",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InPatientWaitlistIsOpen",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OutPatientWaitlistIsOpen",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InPatientWaitlistIsOpen",
                table: "MentalHealthData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OutPatientWaitlistIsOpen",
                table: "MentalHealthData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WaitlistIsOpen",
                table: "JobTrainingData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmergencyPrivateBedsWaitlistIsOpen",
                table: "HousingData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmergencySharedBedsWaitlistIsOpen",
                table: "HousingData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LongTermPrivateBedsWaitlistIsOpen",
                table: "HousingData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LongTermSharedBedsWaitlistIsOpen",
                table: "HousingData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WaitlistIsOpen",
                table: "CaseManagementData",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetoxWaitlistIsOpen",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "GroupWaitlistIsOpen",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "InPatientWaitlistIsOpen",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "OutPatientWaitlistIsOpen",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "InPatientWaitlistIsOpen",
                table: "MentalHealthData");

            migrationBuilder.DropColumn(
                name: "OutPatientWaitlistIsOpen",
                table: "MentalHealthData");

            migrationBuilder.DropColumn(
                name: "WaitlistIsOpen",
                table: "JobTrainingData");

            migrationBuilder.DropColumn(
                name: "EmergencyPrivateBedsWaitlistIsOpen",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "EmergencySharedBedsWaitlistIsOpen",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "LongTermPrivateBedsWaitlistIsOpen",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "LongTermSharedBedsWaitlistIsOpen",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "WaitlistIsOpen",
                table: "CaseManagementData");

            migrationBuilder.AddColumn<int>(
                name: "DetoxWaitlistLength",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupWaitlistLength",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InPatientWaitlistLength",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OutPatientWaitlistLength",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InPatientWaitlistLength",
                table: "MentalHealthData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OutPatientWaitlistLength",
                table: "MentalHealthData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaitlistLength",
                table: "JobTrainingData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmergencyPrivateBedsWaitlistLength",
                table: "HousingData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmergencySharedBedsWaitlistLength",
                table: "HousingData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongTermPrivateBedsWaitlistLength",
                table: "HousingData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongTermSharedBedsWaitlistLength",
                table: "HousingData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaitlistLength",
                table: "CaseManagementData",
                nullable: false,
                defaultValue: 0);
        }
    }
}
