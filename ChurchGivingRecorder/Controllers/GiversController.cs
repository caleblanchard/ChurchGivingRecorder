﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChurchGivingRecorder.Data;
using ChurchGivingRecorder.Models;
using NonFactors.Mvc.Lookup;
using Microsoft.AspNetCore.Authorization;

namespace ChurchGivingRecorder.Controllers
{
    [Authorize]
    public class GiversController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiversController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Givers
        public IActionResult Index()
        {
            return View(_context.Givers.OrderBy(g => g.EnvelopeID));
        }

        [HttpGet]
        public JsonResult AllGivers(LookupFilter filter)
        {
            GiverLookup lookup = new GiverLookup(_context);
            lookup.Filter = filter;

            return Json(lookup.GetData());
        }

        // GET: Givers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giver = await _context.Givers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (giver == null)
            {
                return NotFound();
            }

            return View(giver);
        }

        // GET: Givers/Create
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: Givers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EnvelopeID,Name,Address,NeedLetter,NeedBox")] Giver giver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView(giver);
        }

        // GET: Givers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giver = await _context.Givers.FindAsync(id);
            if (giver == null)
            {
                return NotFound();
            }
            return PartialView(giver);
        }

        // POST: Givers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnvelopeID,Name,Address,NeedLetter,NeedBox")] Giver giver)
        {
            if (id != giver.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiverExists(giver.Id))
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
            return PartialView(giver);
        }

        // GET: Givers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giver = await _context.Givers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (giver == null)
            {
                return NotFound();
            }

            return PartialView(giver);
        }

        // POST: Givers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giver = await _context.Givers.FindAsync(id);
            _context.Givers.Remove(giver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiverExists(int id)
        {
            return _context.Givers.Any(e => e.Id == id);
        }
    }
}
