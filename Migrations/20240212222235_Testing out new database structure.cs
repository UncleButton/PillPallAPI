using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillPallAPI.Migrations
{
    /// <inheritdoc />
    public partial class Testingoutnewdatabasestructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMeds_TestSchedules_TestScheduleId",
                table: "ScheduleMeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Times_TestSchedules_TestScheduleId",
                table: "Times");

            migrationBuilder.DropTable(
                name: "ScheduleTimes");

            migrationBuilder.DropTable(
                name: "TestSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Times_TestScheduleId",
                table: "Times");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleMeds",
                table: "ScheduleMeds");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleMeds_TestScheduleId",
                table: "ScheduleMeds");

            migrationBuilder.DropColumn(
                name: "TestScheduleId",
                table: "Times");

            migrationBuilder.DropColumn(
                name: "TestScheduleId",
                table: "ScheduleMeds");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Times",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ScheduleMeds",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleMeds",
                table: "ScheduleMeds",
                columns: new[] { "ScheduleId", "MedId" });

            migrationBuilder.CreateIndex(
                name: "IX_Times_ScheduleId",
                table: "Times",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleMeds_MedId",
                table: "ScheduleMeds",
                column: "MedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMeds_Medications_MedId",
                table: "ScheduleMeds",
                column: "MedId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMeds_Schedules_ScheduleId",
                table: "ScheduleMeds",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Times_Schedules_ScheduleId",
                table: "Times",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMeds_Medications_MedId",
                table: "ScheduleMeds");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMeds_Schedules_ScheduleId",
                table: "ScheduleMeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Times_Schedules_ScheduleId",
                table: "Times");

            migrationBuilder.DropIndex(
                name: "IX_Times_ScheduleId",
                table: "Times");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleMeds",
                table: "ScheduleMeds");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleMeds_MedId",
                table: "ScheduleMeds");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Times");

            migrationBuilder.AddColumn<int>(
                name: "TestScheduleId",
                table: "Times",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ScheduleMeds",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "TestScheduleId",
                table: "ScheduleMeds",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleMeds",
                table: "ScheduleMeds",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ScheduleTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PIN = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSchedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Times_TestScheduleId",
                table: "Times",
                column: "TestScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleMeds_TestScheduleId",
                table: "ScheduleMeds",
                column: "TestScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMeds_TestSchedules_TestScheduleId",
                table: "ScheduleMeds",
                column: "TestScheduleId",
                principalTable: "TestSchedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Times_TestSchedules_TestScheduleId",
                table: "Times",
                column: "TestScheduleId",
                principalTable: "TestSchedules",
                principalColumn: "Id");
        }
    }
}
