using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ExpenseTracker.Identity.Dtos;
using ExpenseTracker.Identity.Entities;
using ExpenseTracker.Identity.Infrastructure.Entities;

namespace ExpenseTracker.Identity.Infrastructure.Profiles
{
    internal class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterUserDto, ApplicationUser>();
            CreateMap<RegisterUserDto, ApplicationUserDbEntity>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<ApplicationUser, ApplicationUserDbEntity>().ReverseMap();
        }
    }
}
