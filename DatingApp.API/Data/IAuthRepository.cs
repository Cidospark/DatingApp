using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
        // User register method to return a task
         Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<User> UserExists(string username);
    }
}