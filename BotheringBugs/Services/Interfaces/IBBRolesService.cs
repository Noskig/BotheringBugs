using BotheringBugs.Models;

namespace BotheringBugs.Services.Interfaces
{
    public interface IBBRolesService
    {
        public Task<bool> IsUserInRoleAsync(BBUser user, string roleName);
        public Task<IEnumerable<string>> GetUserRolesAsync(BBUser user);
        public Task<bool> AddUserToRoleAsync(BBUser user, string roleName);
        public Task<bool> RemoveUserFromRolesAsync(BBUser user, string roleName);
        public Task<bool> RemoveUserFromRolesAsync(BBUser user, IEnumerable<string> roles);
        public Task<List<BBUser>> GetUserInRoleAsync(string roleName, int companyId);
        public Task<List<BBUser>> GetUserNotInRoleAsync(string roleName, int companyId);
        public Task<List<BBUser>> GetUsersInRoleAsync(string roleName, int companyId);

        public Task<string> GetRoleNameByIdAsync(string roleId);
    }
}
