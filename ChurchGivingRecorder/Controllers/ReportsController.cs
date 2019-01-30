using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ChurchGivingRecorder.Data;
using ChurchGivingRecorder.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChurchGivingRecorder.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Funds()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Funds(FundsReportParams model)
        {
            FundsReportData reportData = new FundsReportData()
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                FundTotals = new List<FundTotals>()
            };

            foreach (var fund in _context.Funds)
            {
                var fundTotal = new FundTotals()
                {
                    FundName = fund.Name,
                    Data = new Dictionary<string, decimal>()
                };

                switch (model.GroupBy)
                {
                    case GroupBy.Day:

                        var fundDayTotalQuery = from gd in _context.GiftDetails
                                             join g in _context.Gifts on gd.GiftId equals g.Id
                                             where gd.FundId == fund.Id
                                                && g.GiftDate >= model.StartDate
                                                && g.GiftDate <= model.EndDate
                                             group new { g, gd } by new { g.GiftDate } into n
                                             select new
                                             {
                                                 n.Key.GiftDate,
                                                 Sum = n.Sum(x => x.gd.Amount),
                                             };

                        foreach (var fundTotalRecord in fundDayTotalQuery)
                        {
                            fundTotal.Data.Add(fundTotalRecord.GiftDate.ToString("MM/dd/yyyy"), fundTotalRecord.Sum);
                        }

                        reportData.FundTotals.Add(fundTotal);
                        break;

                    case GroupBy.Month:

                        var fundMonthTotalQuery = from gd in _context.GiftDetails
                                             join g in _context.Gifts on gd.GiftId equals g.Id
                                             where gd.FundId == fund.Id
                                                && g.GiftDate >= model.StartDate
                                                && g.GiftDate <= model.EndDate
                                             group new { g, gd } by new { g.GiftDate.Month } into n
                                             select new
                                             {
                                                 n.Key.Month,
                                                 Sum = n.Sum(x => x.gd.Amount),
                                             };

                        foreach (var fundTotalRecord in fundMonthTotalQuery)
                        {
                            fundTotal.Data.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fundTotalRecord.Month), fundTotalRecord.Sum);
                        }

                        reportData.FundTotals.Add(fundTotal);

                        break;

                    case GroupBy.Year:

                        var fundYearTotalQuery = from gd in _context.GiftDetails
                                             join g in _context.Gifts on gd.GiftId equals g.Id
                                             where gd.FundId == fund.Id
                                                && g.GiftDate >= model.StartDate
                                                && g.GiftDate <= model.EndDate
                                             group new { g, gd } by new { g.GiftDate.Year } into n
                                             select new
                                             {
                                                 n.Key.Year,
                                                 Sum = n.Sum(x => x.gd.Amount),
                                             };

                        foreach (var fundTotalRecord in fundYearTotalQuery)
                        {
                            fundTotal.Data.Add(fundTotalRecord.Year.ToString(), fundTotalRecord.Sum);
                        }

                        reportData.FundTotals.Add(fundTotal);
                        break;
                }
            }
            

            return View("FundsReport", reportData);
        }
    }
}