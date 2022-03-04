using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Models.Enums;
using BotheringBugs.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BotheringBugs.Services
{
    public class BBProjectService : IBBProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BBUser> _userManager;

        private readonly IBBRolesService _rolesService;

        public BBProjectService(ApplicationDbContext context,
                                UserManager<BBUser> userManager,
                                IBBRolesService roleService)
        {
            _context = context;
            _userManager = userManager;

            _rolesService = roleService;
        }
        // CRUD - Create
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BBUser currentPM = await GetProjectManagerAsync(projectId);

            //remov the current PM if necessary
            if(currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error - error removing current PM. {ex.Message}");
                    return false;
                }
            }
            try
            {
                await AddProjectManagerAsync(userId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error Adding PM. {ex.Message}");
                return false;
            }


        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BBUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        // CRUD - Archive (Delete)
        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;

            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BBUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BBUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<BBUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<BBUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<BBUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = new();

            projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                                .Include(p => p.Members)
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
            return projects;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);
            int priorityId = await LookupProjectPriorityId(priorityName);

            return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();

        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            return projects.Where(p => p.Archived == true).ToList();
        }

        public async Task<List<BBUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        // CRUD - GetbyId
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project result = await _context.Projects
                                    .Include(p => p.Tickets)
                                    .Include(p => p.Members)
                                    .Include(p => p.ProjectPriority)
                                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
            return result;
        }

        public async Task<BBUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(p=>p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (BBUser member in project?.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }

            }
            return null;

        }

        public async Task<List<BBUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects.Include(p => p.Members)
                                                .FirstOrDefaultAsync(p => p.Id == projectId);
            List<BBUser> members = new();

            foreach (var user in project.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }
            return members;
        }

        public async Task<List<BBUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Company)
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Members)
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Tickets)
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Tickets)
                                        .ThenInclude(t => t.DevelopeerUser)
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Tickets)
                                        .ThenInclude(t => t.OwnerUser)
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Tickets)
                                        .ThenInclude(t => t.TicketPriority)
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Tickets)
                                        .ThenInclude(t => t.TicketStatus)
                                .Include(u => u.Projects)
                                    .ThenInclude(p => p.Tickets)
                                        .ThenInclude(p => p.TicketType)
                                .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error getting user project list. --> {ex.Message}");
                throw;
            }
        }

        public async Task<List<BBUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BBUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id == projectId)).ToListAsync();

            return users.Where(u => u.CompanyId == companyId).ToList();

        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
            bool result = false;

            if (project != null)
            {
                result = project.Members.Any(m => m.Id == userId);
            }
            return result;

        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;
            return priorityId;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(p=>p.Members)
                                                        .FirstOrDefaultAsync(p=> p.Id == projectId);
            try
            {
                foreach (BBUser member in project?.Members)
                {
                    if(await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BBUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {

                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - Error Removing User from project. --> { ex.Message}");
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BBUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BBUser bbUser in members)
                {
                    try
                    {
                        project.Members.Remove(bbUser);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - Error Removing user from project --> {ex.Message}");
                throw;
            }
        }

        // CRUD 
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
