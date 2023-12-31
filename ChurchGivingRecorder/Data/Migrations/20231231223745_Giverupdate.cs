using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChurchGivingRecorder.Data.Migrations
{
    /// <inheritdoc />
    public partial class Giverupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedBox",
                table: "Givers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedLetter",
                table: "Givers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedBox",
                table: "Givers");

            migrationBuilder.DropColumn(
                name: "NeedLetter",
                table: "Givers");
        }
    }
}
