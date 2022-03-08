using BotheringBugs.Models;

namespace BotheringBugs.Services.Interfaces
{
    public interface IBBLookUpService
    {
        public Task<List<TicketPriority>> GetTicketPriorityAsync();
        public Task<List<TicketStatus>> GetTicketStatusAsync();
        public Task<List<TicketType>> GetTicketTypeAsync();
        public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
    }
}
