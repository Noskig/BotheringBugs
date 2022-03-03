using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BotheringBugs.Services
{
    public class BBRoleService : IBBRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BBUser> _userManager;


        public BBRoleService(ApplicationDbContext context, 
                              RoleManager<IdentityRole> roleManager,
                              UserManager<BBUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<bool> AddUserToRoleAsync(BBUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
            return result;
        }

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId);
            string result = await _roleManager.GetRoleNameAsync(role);

            return result;
        }

        public async Task<List<BBUser>> GetUserInRoleAsync(string roleName, int companyId)
        {
            List<BBUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<BBUser> result = users.Where(u => u.CompanyId == companyId).ToList();

            return result;
        }

        public async Task<List<BBUser>> GetUserNotInRoleAsync(string roleName, int companyId)
        {
            List<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();
            List<BBUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList();

            List<BBUser> result = roleUsers.Where(u => u.CompanyId == companyId).ToList();

            return result;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(BBUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);

            return result;
        }

        public async Task<List<BBUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<BBUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<BBUser> result = users.Where(u => u.CompanyId == companyId).ToList();

            return result;
        }

        public async Task<bool> IsUserInRoleAsync(BBUser user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName);

            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BBUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BBUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;

            return result;
        }
    }
}
