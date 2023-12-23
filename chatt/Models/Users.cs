using System.ComponentModel.DataAnnotations.Schema;

namespace chatt.Models
{
    public class Users
    {
        public Users() 
        {
            Messages = new HashSet<Messages>();
            Groups = new HashSet<Groups>();
            GroupUsers = new HashSet<GroupUsers>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }

        //[InverseProperty(nameof(chatt.Models.Messages.User))]
        public virtual ICollection<Messages> Messages { get; set; }
        public virtual ICollection<Groups> Groups { get; set; }
        public virtual ICollection<GroupUsers> GroupUsers { get; set; }

    }
}
