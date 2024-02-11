using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillPallAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddnamefieldtoScheduletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContainerMedMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MedId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerMedMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DispenseLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    MedId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumPills = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispenseLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    NumPills = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpirationDate = table.Column<string>(type: "TEXT", nullable: false),
                    PIN = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxPillsPerDose = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxDosesPerDay = table.Column<int>(type: "INTEGER", nullable: true),
                    PrescriberName = table.Column<string>(type: "TEXT", nullable: true),
                    PharmacyAddr1 = table.Column<string>(type: "TEXT", nullable: true),
                    PharmacyAddr2 = table.Column<string>(type: "TEXT", nullable: true),
                    PharmacyCity = table.Column<string>(type: "TEXT", nullable: true),
                    PharmacyState = table.Column<string>(type: "TEXT", nullable: true),
                    PharmacyZip = table.Column<string>(type: "TEXT", nullable: true),
                    PharmacyPhone = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleMeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    MedId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumPills = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleMeds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

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
                name: "Times",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    PIN = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerMedMaps");

            migrationBuilder.DropTable(
                name: "Containers");

            migrationBuilder.DropTable(
                name: "DispenseLogs");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "ScheduleMeds");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "ScheduleTimes");

            migrationBuilder.DropTable(
                name: "Times");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
