using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillPallAPI.Migrations
{
    /// <inheritdoc />
    public partial class addpillsizetomedicationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "isLarge",
                table: "Medications",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isLarge",
                table: "Medications");
        }
    }
}
