using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BotheringBugs.Data;
using BotheringBugs.Models;
using Microsoft.AspNetCore.Identity;
using BotheringBugs.Extensions;
using BotheringBugs.Models.Enums;
using BotheringBugs.Services.Interfaces;

namespace BotheringBugs.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BBUser> _userManager;
        private readonly IBBProjectService _projectService;
        private readonly IBBLookUpService _lookUpService;
        private readonly IBBTicketService _ticketService;

        public TicketsController(ApplicationDbContext context, UserManager<BBUser> userManager,
            IBBProjectService projectService, IBBLookUpService lookUpService, IBBTicketService ticketService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
            _lookUpService = lookUpService;
            _ticketService = ticketService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {

            var applicationDbContext = _context.Tickets.Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> MyTickets()
        {
            BBUser user = await _userManager.GetUserAsync(User);

            List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(user.Id, user.CompanyId);

            
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            BBUser user = await _userManager.GetUserAsync(User);
            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(user.Id), "Id", "Name");

            }



            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPriorityAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypeAsync(), "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            BBUser user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {

                ticket.Created = DateTimeOffset.Now;
                ticket.OwnerUserId = user.Id;

                ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(BBTicketStatus.New))).Value;

                await _ticketService.AddNewTicketAsync(ticket);

                //TODO: Ticket History

                //TODO: Ticket notification


                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(user.CompanyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(user.Id), "Id", "Name");

            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPriorityAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypeAsync(), "Id", "Name");

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            
            if (ticket == null)
            {
                return NotFound();
            }

            
            
            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPriorityAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookUpService.GetTicketStatusAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypeAsync(), "Id", "Name", ticket.TicketTypeId);
            
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                BBUser user = await _userManager.GetUserAsync(User);

                try
                {
                    ticket.Updated = DateTimeOffset.Now;
                    await _ticketService.UpdateTicketAsync(ticket);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //TODO: Add ticket history

                return RedirectToAction(nameof(Index));
            }


            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPriorityAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookUpService.GetTicketStatusAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypeAsync(), "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);


            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            ticket.Archived = true;
            await _ticketService.UpdateTicketAsync(ticket);

            return RedirectToAction(nameof(Index));
        }

        // GET: Tickets/Restore/5
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);


            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            ticket.Archived = false;
            await _ticketService.UpdateTicketAsync(ticket);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);

        }
    }
}
