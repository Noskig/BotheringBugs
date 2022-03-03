using BotheringBugs.Models;

namespace BotheringBugs.Services.Interfaces
{
    public interface IBBCompanyInfoService
    {
        public Task<Company> GetCompanyInfoByIdAsync(int? companyId);
        public Task<List<BBUser>> GetAllMembersAsync(int companyId);
        public Task<List<Project>> GetAllProjectsAsync(int companyId);
        public Task<List<Ticket>> GetAllTicketsAsync(int companyId);

    }
}
