using ExpenseTracker.Identity.Dtos;
using ExpenseTracker.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Identity.Services
{
    public interface IIdentiyUserService
    {
        Task Register(RegisterUserDto user);

        Task<string> GetToken(LoginUserDto user);
    }
}
