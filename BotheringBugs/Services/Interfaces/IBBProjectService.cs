using BotheringBugs.Models;

namespace BotheringBugs.Services.Interfaces
{
    public interface IBBProjectService
    {
        public Task AddNewProjectAsync(Project project);

        public Task<bool> AddProjectManagerAsync(string userId, int projectId);

        public Task<bool> AddUserToProjectAsync(string userId, int projectId);

        public Task ArchiveProjectAsync(Project project);

        public Task<List<Project>> GetAllProjectsByCompany(int companyId);

        public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName);

        public Task<List<BBUser>> GetAllProjectMembersExceptPMAsync(int projectId);

        public Task<List<Project>> GetArchivedProjectsByCompany(int companyId);

        public Task<List<BBUser>> GetDevelopersOnProjectAsync(int projectId);

        public Task<BBUser> GetProjectManagerAsync(int projectId);

        public Task<List<BBUser>> GetProjectMembersByRoleAsync(int projectId, string role);

        public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

        public Task<List<BBUser>> GetSubmittersOnProjectAsync(int projectId);

        public Task<List<BBUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);

        public Task<List<Project>> GetUserProjectsAsync(string userId);

        public Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId);

        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        public Task<int> LookupProjectPriorityId(string priorityName);

        public Task RemoveProjectManagerAsync(int projectId);

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

        public Task RemoveUserFromProjectAsync(string userId, int projectId);

        public Task RestoreProjectAsync(Project project);
        public Task UpdateProjectAsync(Project project);
    }
}
