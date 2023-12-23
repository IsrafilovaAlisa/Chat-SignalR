namespace chatt.Models
{
    public class GroupUsers
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public Groups Group { get; set; }

    }
}
