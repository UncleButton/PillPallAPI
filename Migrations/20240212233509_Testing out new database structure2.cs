using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillPallAPI.Migrations
{
    /// <inheritdoc />
    public partial class Testingoutnewdatabasestructure2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMeds_Medications_MedId",
                table: "ScheduleMeds");

            migrationBuilder.RenameColumn(
                name: "MedId",
                table: "ScheduleMeds",
                newName: "MedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleMeds_MedId",
                table: "ScheduleMeds",
                newName: "IX_ScheduleMeds_MedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMeds_Medications_MedicationId",
                table: "ScheduleMeds",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMeds_Medications_MedicationId",
                table: "ScheduleMeds");

            migrationBuilder.RenameColumn(
                name: "MedicationId",
                table: "ScheduleMeds",
                newName: "MedId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleMeds_MedicationId",
                table: "ScheduleMeds",
                newName: "IX_ScheduleMeds_MedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMeds_Medications_MedId",
                table: "ScheduleMeds",
                column: "MedId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
