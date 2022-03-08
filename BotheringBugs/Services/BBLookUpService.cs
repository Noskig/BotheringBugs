using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BotheringBugs.Services
{
    public class BBLookUpService : IBBLookUpService
    {
        private readonly ApplicationDbContext _context;

        public BBLookUpService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                return await _context.ProjectPriorities.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketPriority>> GetTicketPriorityAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketStatus>> GetTicketStatusAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketType>> GetTicketTypeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
