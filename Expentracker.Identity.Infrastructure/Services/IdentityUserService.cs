using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ExpenseTracker.Identity.Dtos;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ExpenseTracker.Identity.Infrastructure.Entities;
using ExpenseTracker.Identity.Services;
using ExpenseTracker.Identity.Common.Exceptions;

namespace ExpenseTracker.Identity.Infrastructure.Services
{
    public class IdentityUserService : IIdentiyUserService
    {
        private readonly UserManager<ApplicationUserDbEntity> _userManager;
        private readonly SignInManager<ApplicationUserDbEntity> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public IdentityUserService(UserManager<ApplicationUserDbEntity> userManager, SignInManager<ApplicationUserDbEntity> signInManager, IMapper mapper
            , IConfiguration configuration)
        { 
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task Register(RegisterUserDto user)
        {
            var result = await _userManager.CreateAsync(_mapper.Map<ApplicationUserDbEntity>(user), user.Password);
            if (!result.Succeeded)
                throw new Exception("Unable to create user");
        }

        public async Task<string> GetToken(LoginUserDto loginUserDto)
        {
            var user = await this._userManager.Users.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email);
            if (user == null)
                return null;

            var validPassword = await this._userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!validPassword)
                throw new AuthenticationException();

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfiguration:Issuer"],
                audience: _configuration["JwtConfiguration:Audience"],
                expires: DateTime.Now.AddHours(24 * int.Parse(_configuration["JwtConfiguration:ExpireDays"])),
                claims: new List<Claim>()
                        {
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim("FirstName", user.Firstname),
                            new Claim("LastName", user.Lastname)
                        },
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(@_configuration["JwtConfiguration:Secret"])), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
