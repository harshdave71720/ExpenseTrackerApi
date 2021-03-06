using ExpenseTracker.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;
using ExpenseTracker.Core.Exceptions;

namespace ExpenseTracker.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            Guard.AgainstDependencyNull(userRepository);
            _userRepository = userRepository;
        }

        public async Task<User> Add(User user)
        {
            Guard.AgainstNull(user, nameof(user));
            if (await _userRepository.Exists(user.Email))
                throw new BadRequestException(ErrorMessages.UserAlreadyExists(user.Email));

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
