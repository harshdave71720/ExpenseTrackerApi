using System;
using System.Collections.Generic;
using System.Text;
using ExpenseTracker.Core.Repositories;
using System.Threading.Tasks;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Persistence.DbContexts;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Persistence.Entities;

namespace ExpenseTracker.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ExpenseDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserRepository(ExpenseDbContext dbContext, IMapper mapper)
        { 
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
        
        public async Task<User> GetUser(string email)
        {
            return _mapper.Map<User>(await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email)));
        }

        public async Task<User> Add(User user)
        {
            var userToAdd = _mapper.Map<UserEntity>(user);
            _dbContext.Attach(userToAdd);
            _dbContext.Entry(userToAdd).State = EntityState.Added;
            await this.SaveChangesAsync();
            return _mapper.Map<User>(userToAdd);
        }

        public async Task<bool> Exists(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
