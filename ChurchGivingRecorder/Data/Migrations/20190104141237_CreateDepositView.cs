using Microsoft.EntityFrameworkCore.Migrations;

namespace ChurchGivingRecorder.Data.Migrations
{
    public partial class CreateDepositView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#if MYSQL
            migrationBuilder.Sql(@"CREATE VIEW `DepositView`
AS
SELECT A.Id, A.DepositDate, A.Description, IFNULL(Sum(C.Amount),0) As TotalAmount FROM Deposits A
LEFT JOIN Gifts B ON A.Id = B.DepositId
LEFT JOIN GiftDetails C ON B.Id = C.GiftId
GROUP BY A.Id, A.DepositDate, A.Description");
#else
            migrationBuilder.Sql(@"CREATE VIEW [dbo].[DepositView]
AS
SELECT A.Id, A.DepositDate, A.Description, ISNULL(Sum(C.Amount),0) As TotalAmount FROM dbo.Deposits A
LEFT JOIN dbo.Gifts B ON A.Id = B.DepositId
LEFT JOIN dbo.GiftDetails C ON B.Id = C.GiftId
GROUP BY A.Id, A.DepositDate, A.Description");
#endif
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW [dbo].[DepositView]");
        }
    }
}
