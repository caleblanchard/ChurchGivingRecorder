using Microsoft.EntityFrameworkCore.Migrations;

namespace ChurchGivingRecorder.Data.Migrations
{
    public partial class AddEnvelopeIdString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnvelopIdString",
                table: "Givers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvelopIdString",
                table: "Givers");
        }
    }
}
