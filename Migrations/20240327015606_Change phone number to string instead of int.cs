using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillPallAPI.Migrations
{
    /// <inheritdoc />
    public partial class Changephonenumbertostringinsteadofint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PharmacyPhone",
                table: "Medications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PharmacyPhone",
                table: "Medications",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
