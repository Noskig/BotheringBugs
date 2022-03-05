using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BotheringBugs.Services
{
    public class BBNotificationService : IBBNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBBRolesService _roleService;

        public BBNotificationService(ApplicationDbContext context,
                                IEmailSender emailSender,
                                IBBRolesService roleService)
        {
            _context = context;
            _emailSender = emailSender;
            _roleService = roleService;
        }

        public IBBRolesService RoleService { get; }

        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error adding notification: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Notification>> GetRecievedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                                                .Include(n => n.Recipient)
                                                                .Include(n => n.Sender)
                                                                .Include(n => n.Ticket)
                                                                    .ThenInclude(t => t.Project)
                                                                .Where(n => n.RecipientId == userId).ToListAsync();

                return notifications;
                                                                
            }
            catch (Exception ex )
            {
                Console.WriteLine($"Error - error getting notification: {ex.Message}");

                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                                                .Include(n => n.Recipient)
                                                                .Include(n => n.Sender)
                                                                .Include(n => n.Ticket)
                                                                    .ThenInclude(t => t.Project)
                                                                .Where(n => n.SenderId == userId).ToListAsync();

                return notifications;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error getting notification: {ex.Message}");

                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            BBUser bbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

            if(bbUser != null)
            {
                string bbUserEmail = bbUser.Email;
                string message = notification.Message;

                //Send email
                try
                {
                    await _emailSender.SendEmailAsync(bbUserEmail, emailSubject, message);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error - error sending email: {ex.Message}");

                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task SendEmailNotificationByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<BBUser> members = await _roleService.GetUserInRoleAsync(role, companyId);

                foreach (BBUser bbUser in members)
                {
                    notification.RecipientId = bbUser.Id;

                    await SendEmailNotificationAsync(notification, notification.Title);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error sending email to members in roles: {ex.Message}");
                throw;
            }
        }

        public async Task SendMembersEmailNotificationAsync(Notification notification, List<BBUser> members)
        {
            try
            {
                foreach (BBUser bbUser in members)
                {
                    notification.RecipientId = bbUser.Id;

                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error sending email to list: {ex.Message}");

                throw;
            }
        }
    }
}
