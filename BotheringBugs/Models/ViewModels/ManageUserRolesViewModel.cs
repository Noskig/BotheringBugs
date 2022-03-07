using Microsoft.AspNetCore.Mvc.Rendering;

namespace BotheringBugs.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BBUser BBUser { get; set; }
        public MultiSelectList Roles { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}
