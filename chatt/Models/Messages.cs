namespace chatt.Models
{
    public class Messages
    {
        public int Id { get; set; }
        public int UserId { get ; set; }
        public string Text { get; set; }
        public DateTime DateStamp { get; set; }

        public int GroupId { get; set; }
        public Users User { get; set; }
        public Groups Group { get; set; }
    }
}
