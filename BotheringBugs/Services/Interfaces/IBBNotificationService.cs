using BotheringBugs.Models;

namespace BotheringBugs.Services.Interfaces
{
    public interface IBBNotificationService
    {
        public Task AddNotificationAsync(Notification notification);
        public Task<List<Notification>> GetRecievedNotificationsAsync(string userId);
        public Task<List<Notification>> GetSentNotificationsAsync(string userId);
        public Task SendEmailNotificationByRoleAsync(Notification notification, int companyId, string role);
        public Task SendMembersEmailNotificationAsync (Notification notification, List<BBUser> members);
        public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);
    }
}
