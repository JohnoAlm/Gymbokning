using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Gymbokning.ViewModels;

namespace Gymbokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(IMapper mapper, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null) return NotFound();

            //var gymClass = await _context.GymClass.FindAsync(id);

            var userId = _userManager.GetUserId(User);

            var attending = await _context.ApplicationUserGymClass.FindAsync(userId, id);

            // om den inte finns
            // skapa en new ApplicationUserGymClass och ge den userId och id
            if(attending == null)
            {
                 var applicationUserGymClass = new ApplicationUserGymClass
                 {
                     ApplicationUserId = userId,
                     GymClassId = (int)id

                 };

                _context.Add(applicationUserGymClass);
            }

            //om raden i kopplingstabellen finns som har userId och id
            // ta bort den raden från tabellen
            else
            {
                _context.Remove(attending);
            }

            // savechanges
            _context.SaveChanges();

            //redirect to action index
            return RedirectToAction(nameof(Index));
        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            //var m = mapper.ProjectTo<GymClassesViewModel>(_context.GymClass).ToListAsync();

            var classes = await _context.GymClass.ToListAsync();
            var mmm = mapper.Map<IEnumerable<GymClassesViewModel>>(classes);


            var userId = _userManager.GetUserId(User);
            var gymClasses2 = await _context.GymClass.Include(g => g.AttendingMembers).ToListAsync();
            var res = mapper.Map<IEnumerable<GymClassesViewModel>>(gymClasses2, opt => opt.Items.Add("Id", userId));

            var gymClasses = await _context.GymClass.Include(g => g.AttendingMembers)
                .Select(g => new GymClassesViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                    Duration = g.Duration,
                    StartDate = g.StartTime,
                    Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)

                })
                .ToListAsync();



            return View(res);
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClass == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        [Authorize (Roles = "Admin")]
        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize (Roles = "Admin")]
        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        [Authorize (Roles = "Admin")]
        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClass == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        [Authorize (Roles = "Admin")]
        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        [Authorize (Roles = "Admin")]
        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymClass == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        [Authorize (Roles = "Admin")]
        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymClass == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GymClass'  is null.");
            }
            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClass.Remove(gymClass);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
          return (_context.GymClass?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
