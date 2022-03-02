using System.ComponentModel;

namespace BotheringBugs.Models
{
    public class TicketType
    {
        public int Id { get; set; }
        [DisplayName("Type Name")]
        public string Name { get; set; }
    }
}
