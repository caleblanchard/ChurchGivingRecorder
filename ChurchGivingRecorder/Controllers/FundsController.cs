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
    public class FundsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FundsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Funds
        public async Task<IActionResult> Index()
        {
            return View(await _context.Funds.ToListAsync());
        }

        // GET: Funds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _context.Funds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fund == null)
            {
                return NotFound();
            }

            return View(fund);
        }

        // GET: Funds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Funds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Fund fund)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fund);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fund);
        }

        // GET: Funds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _context.Funds.FindAsync(id);
            if (fund == null)
            {
                return NotFound();
            }
            return View(fund);
        }

        // POST: Funds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Fund fund)
        {
            if (id != fund.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fund);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FundExists(fund.Id))
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
            return View(fund);
        }

        // GET: Funds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _context.Funds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fund == null)
            {
                return NotFound();
            }

            return View(fund);
        }

        // POST: Funds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fund = await _context.Funds.FindAsync(id);
            _context.Funds.Remove(fund);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FundExists(int id)
        {
            return _context.Funds.Any(e => e.Id == id);
        }
    }
}
