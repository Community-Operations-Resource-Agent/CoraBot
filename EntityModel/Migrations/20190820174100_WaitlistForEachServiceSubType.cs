using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class WaitlistForEachServiceSubType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasWaitlist",
                table: "SubstanceUseData",
                newName: "OutPatientHasWaitlist");

            migrationBuilder.RenameColumn(
                name: "HasWaitlist",
                table: "MentalHealthData",
                newName: "OutPatientHasWaitlist");

            migrationBuilder.RenameColumn(
                name: "HasWaitlist",
                table: "HousingData",
                newName: "LongTermSharedBedsHasWaitlist");

            migrationBuilder.AddColumn<bool>(
                name: "DetoxHasWaitlist",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GroupHasWaitlist",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InPatientHasWaitlist",
                table: "SubstanceUseData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InPatientHasWaitlist",
                table: "MentalHealthData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmergencyPrivateBedsHasWaitlist",
                table: "HousingData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmergencySharedBedsHasWaitlist",
                table: "HousingData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LongTermPrivateBedsHasWaitlist",
                table: "HousingData",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetoxHasWaitlist",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "GroupHasWaitlist",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "InPatientHasWaitlist",
                table: "SubstanceUseData");

            migrationBuilder.DropColumn(
                name: "InPatientHasWaitlist",
                table: "MentalHealthData");

            migrationBuilder.DropColumn(
                name: "EmergencyPrivateBedsHasWaitlist",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "EmergencySharedBedsHasWaitlist",
                table: "HousingData");

            migrationBuilder.DropColumn(
                name: "LongTermPrivateBedsHasWaitlist",
                table: "HousingData");

            migrationBuilder.RenameColumn(
                name: "OutPatientHasWaitlist",
                table: "SubstanceUseData",
                newName: "HasWaitlist");

            migrationBuilder.RenameColumn(
                name: "OutPatientHasWaitlist",
                table: "MentalHealthData",
                newName: "HasWaitlist");

            migrationBuilder.RenameColumn(
                name: "LongTermSharedBedsHasWaitlist",
                table: "HousingData",
                newName: "HasWaitlist");
        }
    }
}
