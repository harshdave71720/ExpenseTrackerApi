using ExpenseTracker.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;

namespace ExpenseTracker.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            Guard.AgainstNull(userRepository, nameof(userRepository));
            _userRepository = userRepository;
        }

        public async Task<User> Add(User user)
        {
            Guard.AgainstNull(user, nameof(user));
            if (await _userRepository.Exists(user.Email))
                return null;

            var addedUser = await _userRepository.Add(user);
            return addedUser;
        }

        public Task<User> Get(string email)
        {
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));
            return _userRepository.GetUser(email);
        }
    }
}
