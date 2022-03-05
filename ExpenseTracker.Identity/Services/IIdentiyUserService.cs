using ExpenseTracker.Identity.Dtos;
using System.Threading.Tasks;

namespace ExpenseTracker.Identity.Services
{
    public interface IIdentiyUserService
    {
        Task Register(RegisterUserDto user);

        Task<string> GetToken(LoginUserDto user);
    }
}
