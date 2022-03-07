using BotheringBugs.Extensions;
using BotheringBugs.Models;
using BotheringBugs.Models.ViewModels;
using BotheringBugs.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BotheringBugs.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IBBRolesService _rolesService;
        private readonly IBBCompanyInfoService _bbCompanyInfoService;
        public UserRolesController(IBBRolesService rolesService, IBBCompanyInfoService bbCompanyInfoService)
        {
            _rolesService = rolesService;
            _bbCompanyInfoService = bbCompanyInfoService;
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            //Add instance 
            List<ManageUserRolesViewModel> model = new();

            //Get companyId
            int companyId = User.Identity.GetCompanyId().Value;
            //Get all company users
            List<BBUser> users = await _bbCompanyInfoService.GetAllMembersAsync(companyId);

            //Loop over the users to populate the ViewModel
            //- instantiate ViewModel
            //- use _rolesService
            //- Creat multi-selects
            foreach (BBUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BBUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);

            }

            //return the model to the view
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            BBUser bbUser = (await _bbCompanyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BBUser.Id);
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(bbUser);

            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole)) 
            {
                if(await _rolesService.RemoveUserFromRolesAsync(bbUser, roles))
                {
                    await _rolesService.AddUserToRoleAsync(bbUser, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
