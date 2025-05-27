using Desktop_Nhom13.Models.Users;

namespace Desktop_Nhom13
{
    public interface IUserContext
    {
        User CurrentUser { get; set; }
    }
    public class UserContext : IUserContext
    {
        public User CurrentUser { get; set; }
    }
}
