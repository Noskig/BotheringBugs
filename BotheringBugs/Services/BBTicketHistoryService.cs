using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BotheringBugs.Services
{
    public class BBTicketHistoryService : IBBTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BBTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userID)
        {
            // New ticket added
            if (oldTicket == null && newTicket != null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTime.Now,
                    UserId = userID,
                    Description = "New ticket Created"
                };
                try
                {
                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                if (oldTicket.Title != newTicket.Title)
                {

                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTime.Now,
                        UserId = userID,
                        Description = $"New ticket Title: {newTicket.Title}"
                    };

                    await _context.TicketHistories.AddAsync(history);
                }
               
                //Check Ticket Descriptiopn
                if(oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTime.Now,
                        UserId = userID,
                        Description = $"New ticket Title: {newTicket.Description}"
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
                
                //Check Ticket Priority
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketPriority",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTime.Now,
                        UserId = userID,
                        Description = $"New ticket Priority: {newTicket.TicketPriority.Name}"
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
                
                //Check Ticket Status
                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketStatus",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTime.Now,
                        UserId = userID,
                        Description = $"New ticket Priority: {newTicket.TicketStatus.Name}"
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
                
                //Check Ticket Type
                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketTypeId",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTime.Now,
                        UserId = userID,
                        Description = $"New ticket Priority: {newTicket.TicketType.Name}"
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
               
                //Check Ticket Developer
                if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.DevelopeerUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DevelopeerUser?.FullName,
                        Created = DateTime.Now,
                        UserId = userID,
                        Description = $"New ticket developer: {newTicket.DevelopeerUser.FullName}"
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
                try
                {
                    //Save the TicketHistory to the database
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error - error trying to save ticket history: {ex.Message}");
                    throw;
                }



            }


        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> projects = (await _context.Companies
                                                        .Include(c => c.Projects)
                                                            .ThenInclude(p => p.Tickets)
                                                                .ThenInclude(t=> t.History)
                                                                    .ThenInclude(h=> h.BBUser)
                                                        .FirstOrDefaultAsync(c => c.Id == companyId)).Projects.ToList();
                List<Ticket> tickets = projects.SelectMany(p => p.Tickets).ToList();

                List<TicketHistory> ticketHistories = tickets.SelectMany(t => t.History).ToList();

                return ticketHistories;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error trying to get ticket history: {ex.Message}");
                throw;
            }
        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(t => t.History)
                                                                    .ThenInclude(h => h.BBUser)
                                                            .FirstOrDefaultAsync(p => p.Id == projectId);

                List<TicketHistory> ticketHistory = project.Tickets.SelectMany(t=> t.History).ToList();

                return ticketHistory;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error trying to get ticket history: {ex.Message}");
                throw;
            }
        }
    }
}
