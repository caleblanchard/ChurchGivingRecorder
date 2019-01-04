﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ChurchGivingRecorder.Data.Migrations
{
    public partial class CreateDepositView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW [dbo].[DepositView]
AS
SELECT A.Id, A.DepositDate, A.Description, Sum(C.Amount) As TotalAmount FROM dbo.Deposits A
LEFT JOIN dbo.Gifts B ON A.Id = B.DepositId
LEFT JOIN dbo.GiftDetails C ON B.Id = C.GiftId
GROUP BY A.Id, A.DepositDate, A.Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW [dbo].[DepositView]");
        }
    }
}
