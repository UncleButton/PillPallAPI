using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillPallAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDaysToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Days",
                table: "Schedules",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Days",
                table: "Schedules");
        }
    }
}
