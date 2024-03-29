﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChurchGivingRecorder.Data;
using ChurchGivingRecorder.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChurchGivingRecorder.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private const int fundTotalsId = Int16.MaxValue;
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        public ReportsController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                Givers = await _context.Givers.OrderBy(g => g.EnvelopeID).ToListAsync()
            };
            return View(giversReportParams);
        }

        [HttpGet]
        public async Task<IActionResult> GiversList()
        {
            List<GiverListData> giverList = new List<GiverListData>();

            var givers = await _context.Givers.OrderBy(g => g.EnvelopeID).ToListAsync();

            foreach(var giver in givers)
            {
                giverList.Add(new GiverListData
                {
                    Id = giver.Id,
                    Name = giver.Name,
                    EnvelopeID = giver.EnvelopeID,
                    NeedBox = giver.NeedBox,
                    NeedLetter = giver.NeedLetter
                });
            }

            return View("GiversListReport", giverList);
        }

        [HttpPost]
        public async Task<IActionResult> Givers(GiversReportParams model)
        {
            GiversReportData reportData = new GiversReportData()
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                GiverTotals = new List<GiverTotals>()
            };

            IQueryable<Giver> givers = _context.Givers.OrderBy(g => g.EnvelopeID);

            if (model.GiverId != 0)
            {
                givers = givers.Where(g => g.Id == model.GiverId);
            }

            List<Giver> giversList = await givers.ToListAsync();

            foreach (var giver in giversList)
            {
                var giverTotal = new GiverTotals()
                {
                    GiverName = giver.EnvelopeNameDisplay,
                    Data = new Dictionary<string, Dictionary<int, decimal>>(),
                    Funds = new  Dictionary<int, string>()
                };

                giverTotal.Funds.Add(fundTotalsId, "Total");

                Dictionary<int, decimal> fundDataList = new Dictionary<int, decimal>();
                decimal groupTotal = 0;
                switch (model.GroupBy)
                {
                    case GroupBy.Day:

                        var giverDayTotalQuery = from f in _context.Funds
                                                join gd in _context.GiftDetails on f.Id equals gd.FundId
                                                join g in _context.Gifts on gd.GiftId equals g.Id
                                                where g.GiverId == giver.Id
                                                   && g.GiftDate >= model.StartDate
                                                   && g.GiftDate <= model.EndDate
                                                group new { f, g, gd } by new { f.Id, f.Name, g.GiftDate }
                                                into n
                                                select new
                                                {
                                                    n.Key.Id,
                                                    n.Key.Name,
                                                    n.Key.GiftDate,
                                                    Sum = n.Sum(x => x.gd.Amount),
                                                };
                        DateTime previousDate = DateTime.MinValue;
                        foreach (var fundTotalRecord in giverDayTotalQuery.OrderBy(g => g.GiftDate))
                        {
                            if (previousDate != DateTime.MinValue && previousDate != fundTotalRecord.GiftDate)
                            {
                                fundDataList.Add(fundTotalsId, groupTotal);
                                giverTotal.Data.Add(previousDate.ToString("MM/dd/yyyy"), fundDataList);
                                fundDataList = new Dictionary<int, decimal>();
                                groupTotal = 0;
                            }
                            giverTotal.Funds.TryAdd(fundTotalRecord.Id, fundTotalRecord.Name);
                            fundDataList.Add(fundTotalRecord.Id, fundTotalRecord.Sum);
                            
                            giverTotal.Total += fundTotalRecord.Sum;
                            groupTotal += fundTotalRecord.Sum;
                            previousDate = fundTotalRecord.GiftDate;
                        }
                        if (previousDate != DateTime.MinValue)
                        {
                            fundDataList.Add(fundTotalsId, groupTotal);
                            giverTotal.Data.Add(previousDate.ToString("MM/dd/yyyy"), fundDataList);
                        }

                        reportData.GiverTotals.Add(giverTotal);
                        break;

                    case GroupBy.Month:

                        var giverMonthTotalQuery = from f in _context.Funds
                                                  join gd in _context.GiftDetails on f.Id equals gd.FundId
                                                  join g in _context.Gifts on gd.GiftId equals g.Id
                                                  where g.GiverId == giver.Id
                                                     && g.GiftDate >= model.StartDate
                                                     && g.GiftDate <= model.EndDate
                                                  group new { f, g, gd } by new { f.Id, f.Name, g.GiftDate.Month } into n
                                                  select new
                                                  {
                                                      n.Key.Id,
                                                      n.Key.Name,
                                                      n.Key.Month,
                                                      Sum = n.Sum(x => x.gd.Amount),
                                                  };

                        int previousMonth = 0;
                        foreach (var fundTotalRecord in giverMonthTotalQuery.OrderBy(g => g.Month))
                        {
                            if (previousMonth != 0 && previousMonth != fundTotalRecord.Month)
                            {
                                fundDataList.Add(fundTotalsId, groupTotal);
                                giverTotal.Data.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(previousMonth), fundDataList);
                                fundDataList = new Dictionary<int, decimal>();
                                groupTotal = 0;
                            }
                            giverTotal.Funds.TryAdd(fundTotalRecord.Id, fundTotalRecord.Name);
                            fundDataList.Add(fundTotalRecord.Id, fundTotalRecord.Sum);
                            
                            giverTotal.Total += fundTotalRecord.Sum;
                            groupTotal += fundTotalRecord.Sum;
                            previousMonth = fundTotalRecord.Month;
                        }
                        if (previousMonth != 0)
                        {
                            fundDataList.Add(fundTotalsId, groupTotal);
                            giverTotal.Data.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(previousMonth), fundDataList);
                        }

                        reportData.GiverTotals.Add(giverTotal);
                        break;

                    case GroupBy.Year:

                        var giverYearTotalQuery = await (from f in _context.Funds
                                                  join gd in _context.GiftDetails on f.Id equals gd.FundId
                                                 join g in _context.Gifts on gd.GiftId equals g.Id
                                                 where g.GiverId == giver.Id
                                                    && g.GiftDate >= model.StartDate
                                                    && g.GiftDate <= model.EndDate
                                                 group new { f, g, gd } by new { f.Id, f.Name, g.GiftDate.Year } into n
                                                 orderby n.Key.Year
                                                 select new
                                                 {
                                                     n.Key.Id,
                                                     n.Key.Name,
                                                     n.Key.Year,
                                                     Sum = n.Sum(x => x.gd.Amount),
                                                 }).ToListAsync();

                        int previousYear = 0;
                        foreach (var fundTotalRecord in giverYearTotalQuery)
                        {
                            if (previousYear != 0 && previousYear != fundTotalRecord.Year)
                            {
                                fundDataList.Add(fundTotalsId, groupTotal);
                                giverTotal.Data.Add(previousYear.ToString(), fundDataList);
                                fundDataList = new Dictionary<int, decimal>();
                                groupTotal = 0;
                            }
                            giverTotal.Funds.TryAdd(fundTotalRecord.Id, fundTotalRecord.Name);
                            fundDataList.Add(fundTotalRecord.Id, fundTotalRecord.Sum);
                            
                            giverTotal.Total += fundTotalRecord.Sum;
                            groupTotal += fundTotalRecord.Sum;
                            previousYear = fundTotalRecord.Year;
                        }
                        if (previousYear != 0)
                        {
                            fundDataList.Add(fundTotalsId, groupTotal);
                            giverTotal.Data.Add(previousYear.ToString(), fundDataList);
                        }

                        reportData.GiverTotals.Add(giverTotal);
                        break;
                }
                reportData.Total += giverTotal.Total;
            }

            return View("GiversReport", reportData);
        }

        public async Task<IActionResult> EndYearLetter()
        {
            int previousYear = DateTime.Now.Year - 1;
            var endOfYearLetterParams = new EndOfYearLetterParams()
            {
                StartDate = new DateTime(previousYear, 1, 1),
                EndDate = new DateTime(previousYear, 12, 31),
                Givers = await _context.Givers.OrderBy(g => g.EnvelopeID).ToListAsync()
            };
            return View(endOfYearLetterParams);
        }

        [HttpPost]
        public async Task<IActionResult> EndYearLetter(EndOfYearLetterParams model)
        {
            var giver = await _context.Givers.SingleAsync(g => g.Id == model.GiverId);

            var fundYearTotalQuery = from gd in _context.GiftDetails
                                     join g in _context.Gifts on gd.GiftId equals g.Id
                                     where g.GiverId == model.GiverId
                                        && g.GiftDate >= model.StartDate
                                        && g.GiftDate <= model.EndDate
                                     group new { g, gd } by new { g.GiftDate.Year } into n
                                     select new
                                     {
                                         n.Key.Year,
                                         Sum = n.Sum(x => x.gd.Amount),
                                     };
            var yearTotal = fundYearTotalQuery.First().Sum;

            var settings = await _context.Settings.FirstOrDefaultAsync();
            //byte[] byteArray = System.IO.File.ReadAllBytes(Path.Combine(_env.WebRootPath, "Content\\GivingLetterTemplate.docx"));
            MemoryStream mem = new MemoryStream();
            {
                mem.Write(settings.EndYearTemplate, 0, settings.EndYearTemplate.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(mem, true))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    var paras = body.Elements<Paragraph>();

                    foreach (var para in paras)
                    {
                        foreach (var run in para.Elements<Run>())
                        {
                            foreach (var text in run.Elements<Text>())
                            {
                                if (text.Text.Contains("GIVERNAME"))
                                {
                                    text.Text = text.Text.Replace("GIVERNAME", giver.Name);
                                }
                                
                                if (text.Text.Contains("FISCALYEAR"))
                                {
                                    text.Text = text.Text.Replace("FISCALYEAR", model.StartDate.Year.ToString());
                                }

                                if (text.Text.Contains("GIFTTOTAL"))
                                {
                                    text.Text = text.Text.Replace("GIFTTOTAL", yearTotal.ToString("C"));
                                }

                                if (text.Text.Contains("ITEMIZED"))
                                {
                                    var items = from gd in _context.GiftDetails
                                                join g in _context.Gifts on gd.GiftId equals g.Id
                                                where g.GiverId == model.GiverId
                                                   && g.GiftDate >= model.StartDate
                                                   && g.GiftDate <= model.EndDate
                                                group new { g, gd } by new { g.GiftDate } into n
                                                where n.Sum(x => x.gd.Amount) >= (decimal)250.00
                                                select new
                                                {
                                                    n.Key,
                                                    Sum = n.Sum(x => x.gd.Amount),
                                                };

                                    StringBuilder sb = new StringBuilder();
                                    text.Text = "";
                                    bool isFirst = true;
                                    foreach (var giftDetail in items)
                                    {
                                        if (!isFirst)
                                        {
                                            text.InsertBeforeSelf(new Break());
                                        }
                                        else
                                        {
                                            isFirst = false;
                                        }
                                        text.InsertBeforeSelf(new Text(giftDetail.Key.GiftDate.ToString("MM/dd/yy")));
                                        text.InsertBeforeSelf(new TabChar());
                                        text.InsertBeforeSelf(new Text(giftDetail.Sum.ToString("C")));
                                    }
                                    text.Remove();
                                }
                            }
                        }
                    }
                }
                if (mem.CanSeek)
                {
                    mem.Seek(0, SeekOrigin.Begin);
                }
                string fileName = $"{giver.EnvelopeNameDisplay} - {model.StartDate.Year} - Statement.docx";
                return File(mem, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
        }
    }
}