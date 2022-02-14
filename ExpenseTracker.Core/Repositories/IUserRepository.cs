using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUser(string email);

        Task<User> Add(User user);

        Task<bool> Exists(string email);

        Task SaveChangesAsync();
    }
}
