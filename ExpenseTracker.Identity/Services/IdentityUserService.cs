﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ExpenseTracker.Identity.Entities;
using ExpenseTracker.Identity.Dtos;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Identity.Services
{
    public class IdentityUserService : IIdentiyUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public IdentityUserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper
            , IConfiguration configuration)
        { 
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task Register(RegisterUserDto user)
        {
            var result = await _userManager.CreateAsync(_mapper.Map<ApplicationUser>(user), user.Password);
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
                return null;

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfiguration:Issuer"],
                audience: _configuration["JwtConfiguration:Audience"],
                expires: DateTime.Now.AddHours(24),
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
