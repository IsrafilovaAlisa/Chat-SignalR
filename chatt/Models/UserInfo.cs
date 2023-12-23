namespace chatt.Models
{
    public class UserInfo
    {
        public int id { get; set; }
        public string name { get; set; }

        public UserInfo(Users user)
        {
            id = user.Id;
            name = user.Name;
        }
    }
}
