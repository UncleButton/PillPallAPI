using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillPallAPI.Migrations
{
    /// <inheritdoc />
    public partial class Testingoutanewwaytotietablestogether : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestScheduleId",
                table: "Times",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestScheduleId",
                table: "ScheduleMeds",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TestSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PIN = table.Column<int>(type: "INTEGER", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMeds_TestSchedules_TestScheduleId",
                table: "ScheduleMeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Times_TestSchedules_TestScheduleId",
                table: "Times");

            migrationBuilder.DropTable(
                name: "TestSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Times_TestScheduleId",
                table: "Times");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleMeds_TestScheduleId",
                table: "ScheduleMeds");

            migrationBuilder.DropColumn(
                name: "TestScheduleId",
                table: "Times");

            migrationBuilder.DropColumn(
                name: "TestScheduleId",
                table: "ScheduleMeds");
        }
    }
}
