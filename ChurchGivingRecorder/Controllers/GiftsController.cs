using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChurchGivingRecorder.Data;
using ChurchGivingRecorder.Models;

namespace ChurchGivingRecorder.Controllers
{
    public class GiftsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiftsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Gifts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Gifts.Include(g => g.Deposit).Include(g => g.Giver);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Gifts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gift = await _context.Gifts
                .Include(g => g.Deposit)
                .Include(g => g.Giver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gift == null)
            {
                return NotFound();
            }

            return View(gift);
        }

        // GET: Gifts/Create
        public async Task<IActionResult> Create(int? id)
        {
            GiftViewModel gift = new GiftViewModel();
            if (id != null)
            {
                var deposit = await _context.Deposits.FindAsync(id);

                gift.DepositId = id ?? 0;
                gift.GiftDate = deposit.DepositDate;

                var giftDetails = new List<GiftDetail>();
                int i = 1;
                foreach (var fund in _context.Funds)
                {
                    giftDetails.Add(new GiftDetail { Id = i, FundId = fund.Id, Fund = fund });
                    i++;
                }
                gift.GiftDetails = giftDetails.ToArray();
            }

            return PartialView(gift);
        }

        // POST: Gifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepositId,GiverId,GiftDate,PaymentMethod,CheckNumber,Description,GiftDetails")] GiftViewModel giftViewModel)
        {
            if (ModelState.IsValid)
            {
                var gift = new Gift()
                {
                    DepositId = giftViewModel.DepositId,
                    GiverId = giftViewModel.GiverId,
                    GiftDate = giftViewModel.GiftDate,
                    PaymentMethod = giftViewModel.PaymentMethod,
                    CheckNumber = giftViewModel.CheckNumber,
                    Description = giftViewModel.Description
                };
                _context.Add(gift);
                await _context.SaveChangesAsync();

                foreach (var giftDetail in giftViewModel.GiftDetails)
                {
                    //if (giftDetail.Amount != 0.0)
                    //{
                        GiftDetail giftDetailInsert = new GiftDetail()
                        {
                            GiftId = gift.Id,
                            FundId = giftDetail.FundId,
                            Amount = giftDetail.Amount,
                            Comment = giftDetail.Comment
                        };
                        _context.GiftDetails.Add(giftDetailInsert);
                    //}
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), "Deposits", new { Id = gift.DepositId });
            }

            return PartialView(giftViewModel);
        }

        // GET: Gifts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gift = await _context.Gifts.FindAsync(id);
            
            if (gift == null)
            {
                return NotFound();
            }
            var giftViewModel = new GiftViewModel()
            {
                DepositId = gift.DepositId,
                GiverId = gift.GiverId,
                GiftDate = gift.GiftDate,
                PaymentMethod = gift.PaymentMethod,
                CheckNumber = gift.CheckNumber,
                Description = gift.Description
            };
            giftViewModel.GiftDetails = await _context.GiftDetails.Include(gd => gd.Fund).Where(gd => gd.GiftId == id).ToArrayAsync();
            return PartialView(giftViewModel);
        }

        // POST: Gifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DepositId,GiverId,GiftDate,PaymentMethod,CheckNumber,Description")] GiftViewModel giftViewModel)
        {
            if (id != giftViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var gift = new Gift()
                    {
                        DepositId = giftViewModel.DepositId,
                        GiverId = giftViewModel.GiverId,
                        GiftDate = giftViewModel.GiftDate,
                        PaymentMethod = giftViewModel.PaymentMethod,
                        CheckNumber = giftViewModel.CheckNumber,
                        Description = giftViewModel.Description
                    };
                    _context.Update(gift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftExists(giftViewModel.Id))
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
            return PartialView(giftViewModel);
        }

        // GET: Gifts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gift = await _context.Gifts
                .Include(g => g.Deposit)
                .Include(g => g.Giver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gift == null)
            {
                return NotFound();
            }

            return PartialView(gift);
        }

        // POST: Gifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            var depositId = gift.DepositId;
            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), "Deposits", new { Id = depositId });
        }

        private bool GiftExists(long id)
        {
            return _context.Gifts.Any(e => e.Id == id);
        }
    }
}
