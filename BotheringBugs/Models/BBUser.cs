using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotheringBugs.Models
{
    public class BBUser : IdentityUser
    {
        [Required]
        [Display(Name ="First Name")]
        [PersonalData]
        public string FirstName { get; set; }
        [Required]
        [Display(Name ="Last Name")]
        [PersonalData]
        public string LastName { get; set; }
        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}";}}

    }
}
