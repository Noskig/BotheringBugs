using BotheringBugs.Models;

namespace BotheringBugs.Services.Interfaces
{
    public interface IBBTicketHistoryService
    {
        Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userID);

        Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId);
        Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId);

        public Task<List<TicketAttachment>> GetLatestTicketAttachmentAsync(int ticketId);
    }
}
