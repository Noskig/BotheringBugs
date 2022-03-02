using System.ComponentModel;

namespace BotheringBugs.Models
{
    public class Company
    {
        public int Id { get; set; }
        [DisplayName("Company Name")]
        public string Name { get; set; }
        [DisplayName("Company Description")]
        public string Description{ get; set; } 

        //Navigation prop
        public virtual ICollection<BBUser> Members { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
