using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChurchGivingRecorder.Data;
using ChurchGivingRecorder.Models;
using Microsoft.AspNetCore.Authorization;

namespace ChurchGivingRecorder.Controllers
{
    [Authorize]
    public class DepositsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepositsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Deposits
        public IActionResult Index()
        {
            return View(_context.Set<DepositView>().OrderByDescending(d => d.DepositDate));
        }

        // GET: Deposits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposit = await _context.Deposits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deposit == null)
            {
                return NotFound();
            }

            return PartialView(deposit);
        }

        // GET: Deposits/Create
        public IActionResult Create()
        {
            Deposit deposit = new Deposit()
            {
                DepositDate = DateTime.Today
            };
            return PartialView(deposit);
        }

        // POST: Deposits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DepositDate,Description")] Deposit deposit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deposit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { Id = deposit.Id });
            }
            return PartialView(deposit);
        }

        // GET: Deposits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposit = await _context.Deposits.FindAsync(id);
            if (deposit == null)
            {
                return NotFound();
            }
            var depositViewModel = new DepositViewModel()
            {
                Id = deposit.Id,
                Description = deposit.Description,
                DepositDate = deposit.DepositDate,
                Gifts = await _context.Gifts.Include(g => g.Giver).Where(g => g.DepositId == deposit.Id).ToListAsync()
            };
            
            foreach (var gift in depositViewModel.Gifts)
            {
                gift.TotalAmount = await _context.GiftDetails.Where(gd => gd.GiftId == gift.Id).SumAsync(gd => gd.Amount);
                depositViewModel.TotalAmount += gift.TotalAmount;
            }

            var fundTotalQuery = from gd in _context.GiftDetails
                                 join g in _context.Gifts on gd.GiftId equals g.Id
                                 join f in _context.Funds on gd.FundId equals f.Id
                                 where g.DepositId == id
                                 group new { f, gd } by new { gd.FundId, f.Name } into n
                                 select new
                                 {
                                     n.Key.FundId,
                                     n.Key.Name,
                                     Sum = n.Sum(x => x.gd.Amount),
                                 };
            depositViewModel.FundTotals = new List<FundTotalViewModel>();
            foreach (var fundTotal in fundTotalQuery)
            {
                var fundTotalViewModel = new FundTotalViewModel()
                {
                    Amount = fundTotal.Sum,
                    FundId = fundTotal.FundId,
                    FundName = fundTotal.Name
                };
                depositViewModel.FundTotals.Add(fundTotalViewModel);
            }

            return View(depositViewModel);
        }

        // POST: Deposits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DepositDate,Description")] Deposit deposit)
        {
            if (id != deposit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deposit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositExists(deposit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(deposit);
        }

        // GET: Deposits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposit = await _context.Deposits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deposit == null)
            {
                return NotFound();
            }

            return PartialView(deposit);
        }

        // POST: Deposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deposit = await _context.Deposits.FindAsync(id);
            _context.Deposits.Remove(deposit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepositExists(int id)
        {
            return _context.Deposits.Any(e => e.Id == id);
        }
    }
}
