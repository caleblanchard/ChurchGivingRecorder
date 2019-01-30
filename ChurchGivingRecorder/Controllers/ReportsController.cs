using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ChurchGivingRecorder.Data;
using ChurchGivingRecorder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            int year = DateTime.Now.Year;
            var fundsReportParams = new FundsReportParams()
            {
                StartDate = new DateTime(year, 1, 1),
                EndDate = new DateTime(year, 12, 31)
            };
            return View(fundsReportParams);
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
                            fundTotal.Total += fundTotalRecord.Sum;
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
                            fundTotal.Total += fundTotalRecord.Sum;
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
                            fundTotal.Total += fundTotalRecord.Sum;
                        }

                        reportData.FundTotals.Add(fundTotal);
                        break;
                }
                reportData.Total += fundTotal.Total;
            }
            

            return View("FundsReport", reportData);
        }

        public async Task<IActionResult> Givers()
        {
            int year = DateTime.Now.Year;
            var giversReportParams = new GiversReportParams()
            {
                StartDate = new DateTime(year, 1, 1),
                EndDate = new DateTime(year, 12, 31),
                Givers = await _context.Givers.OrderBy(g => g.EnvelopeNameDisplay).ToListAsync()
            };
            return View(giversReportParams);
        }

        [HttpPost]
        public IActionResult Givers(GiversReportParams model)
        {
            GiversReportData reportData = new GiversReportData()
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                GiverTotals = new List<GiverTotals>()
            };

            IQueryable<Giver> givers = _context.Givers.OrderBy(g => g.EnvelopIdString);

            if (model.GiverId != 0)
            {
                givers = givers.Where(g => g.Id == model.GiverId);
            }

            foreach (var giver in givers)
            {
                var giverTotal = new GiverTotals()
                {
                    GiverName = giver.EnvelopeNameDisplay,
                    FundTotals = new List<FundTotals>(),
                    Data = new Dictionary<string, decimal>()
                };

                switch (model.GroupBy)
                {
                    case GroupBy.Day:

                        var giverDayTotalQuery = from gd in _context.GiftDetails
                                                join g in _context.Gifts on gd.GiftId equals g.Id
                                                where g.GiverId == giver.Id
                                                   && g.GiftDate >= model.StartDate
                                                   && g.GiftDate <= model.EndDate
                                                group new { g, gd } by new { g.GiftDate } into n
                                                select new
                                                {
                                                    n.Key.GiftDate,
                                                    Sum = n.Sum(x => x.gd.Amount),
                                                };

                        foreach (var fundTotalRecord in giverDayTotalQuery)
                        {
                            giverTotal.Data.Add(fundTotalRecord.GiftDate.ToString("MM/dd/yyyy"), fundTotalRecord.Sum);
                            giverTotal.Total += fundTotalRecord.Sum;
                        }

                        foreach (var fund in _context.Funds)
                        {
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
                            var fundTotal = new FundTotals()
                            {
                                FundName = fund.Name,
                                Data = new Dictionary<string, decimal>()
                            };

                            foreach (var fundTotalRecord in fundDayTotalQuery)
                            {
                                fundTotal.Data.Add(fundTotalRecord.GiftDate.ToString("MM/dd/yyyy"), fundTotalRecord.Sum);
                                fundTotal.Total += fundTotalRecord.Sum;
                            }

                            giverTotal.FundTotals.Add(fundTotal);
                        }

                        reportData.GiverTotals.Add(giverTotal);
                        break;

                    case GroupBy.Month:

                        var giverMonthTotalQuery = from gd in _context.GiftDetails
                                                  join g in _context.Gifts on gd.GiftId equals g.Id
                                                  where g.GiverId == giver.Id
                                                     && g.GiftDate >= model.StartDate
                                                     && g.GiftDate <= model.EndDate
                                                  group new { g, gd } by new { g.GiftDate.Month } into n
                                                  select new
                                                  {
                                                      n.Key.Month,
                                                      Sum = n.Sum(x => x.gd.Amount),
                                                  };

                        foreach (var fundTotalRecord in giverMonthTotalQuery)
                        {
                            giverTotal.Data.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fundTotalRecord.Month), fundTotalRecord.Sum);
                            giverTotal.Total += fundTotalRecord.Sum;
                        }

                        reportData.GiverTotals.Add(giverTotal);
                        break;

                    case GroupBy.Year:

                        var giverYearTotalQuery = from gd in _context.GiftDetails
                                                 join g in _context.Gifts on gd.GiftId equals g.Id
                                                 where g.GiverId == giver.Id
                                                    && g.GiftDate >= model.StartDate
                                                    && g.GiftDate <= model.EndDate
                                                 group new { g, gd } by new { g.GiftDate.Year } into n
                                                 select new
                                                 {
                                                     n.Key.Year,
                                                     Sum = n.Sum(x => x.gd.Amount),
                                                 };

                        foreach (var fundTotalRecord in giverYearTotalQuery)
                        {
                            giverTotal.Data.Add(fundTotalRecord.Year.ToString(), fundTotalRecord.Sum);
                            giverTotal.Total += fundTotalRecord.Sum;
                        }

                        reportData.GiverTotals.Add(giverTotal);
                        break;
                }
                reportData.Total += giverTotal.Total;
            }

            return View("GiversReport", reportData);
        }
    }
}