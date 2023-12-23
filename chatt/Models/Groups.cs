namespace chatt.Models
{
    public class Groups
    {
        public Groups()
        {
            GroupUsers = new HashSet<GroupUsers>();
            Messages = new HashSet<Messages>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public virtual ICollection<GroupUsers> GroupUsers { get; set; }
        public virtual ICollection<Messages> Messages { get; set; }  
    }
}
