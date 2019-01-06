using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChurchGivingRecorder.Data.Migrations
{
    public partial class InitialModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deposits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
#if MYSQL
                    .Annotation("MySql:ValueGeneratedOnAdd", true),
#else
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
#endif
                    DepositDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
#if MYSQL
                    .Annotation("MySql:ValueGeneratedOnAdd", true),
#else
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
#endif
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Givers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
#if MYSQL
                    .Annotation("MySql:ValueGeneratedOnAdd", true),
#else
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
#endif
                    EnvelopeID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Givers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropTable(
                name: "Funds");

            migrationBuilder.DropTable(
                name: "Givers");
        }
    }
}
