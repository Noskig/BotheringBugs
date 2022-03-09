using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BotheringBugs.Services
{
    public class BBCompanyInfoService : IBBCompanyInfoService
    {
        private readonly ApplicationDbContext _context;

        public BBCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<BBUser>> GetAllMembersAsync(int companyId)
        {
            List<BBUser> result = new();
            result = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

            return result;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
        {
            List<Ticket> result = new();

            List<Project> projects = await GetAllProjectsAsync(companyId);

            result = projects.SelectMany(p => p.Tickets).ToList();

            return result;

        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new();

            if (companyId != null)
            {
                result = await _context.Companies
                                    .Include(c=>c.Members)
                                    .Include(c=>c.Projects)
                                    .Include(c=>c.Invites)
                                    .FirstOrDefaultAsync(c => c.Id == companyId);
            }
            return result;

        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> result = new();
            result = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                .Include(p => p.Members)
                                                .Include(p => p.Company)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Comments)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Attachments)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.History)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Notifications)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DevelopeerUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.OwnerUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketStatus)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketPriority)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketType)
                                                .Include(p => p.ProjectPriority)
                                                .ToListAsync();

            return result;
        }
    }
}
