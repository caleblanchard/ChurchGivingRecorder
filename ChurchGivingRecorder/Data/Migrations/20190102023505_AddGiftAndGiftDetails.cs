using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChurchGivingRecorder.Data.Migrations
{
    public partial class AddGiftAndGiftDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gifts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepositId = table.Column<int>(nullable: false),
                    GiverId = table.Column<int>(nullable: false),
                    GiftDate = table.Column<DateTime>(nullable: false),
                    PaymentMethod = table.Column<int>(nullable: false),
                    CheckNumber = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gifts_Deposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "Deposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gifts_Givers_GiverId",
                        column: x => x.GiverId,
                        principalTable: "Givers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiftDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GiftId = table.Column<long>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    FundId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftDetails_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiftDetails_Gifts_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Gifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiftDetails_FundId",
                table: "GiftDetails",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftDetails_GiftId",
                table: "GiftDetails",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_DepositId",
                table: "Gifts",
                column: "DepositId");

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_GiverId",
                table: "Gifts",
                column: "GiverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiftDetails");

            migrationBuilder.DropTable(
                name: "Gifts");
        }
    }
}
