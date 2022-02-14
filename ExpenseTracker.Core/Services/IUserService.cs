using System;
using System.Collections.Generic;
using System.Text;
using ExpenseTracker.Core.Entities;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services
{
    public interface IUserService
    {
        Task<User> Add(User user);

        //Task<User> Update(User user);

        //Task<User> Delete(string email);

        Task<User> Get(string email);
    }
}
