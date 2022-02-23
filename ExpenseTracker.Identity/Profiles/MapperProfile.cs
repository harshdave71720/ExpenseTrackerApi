using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ExpenseTracker.Identity.Dtos;
using ExpenseTracker.Identity.Entities;

namespace ExpenseTracker.Identity.Profiles
{
    internal class MapperProfile : Profile
    {
        public MapperProfile()
        { 
            CreateMap<RegisterUserDto, ApplicationUser>().ReverseMap();
        }
    }
}
