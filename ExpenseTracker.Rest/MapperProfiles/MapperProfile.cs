using System;
using AutoMapper;
using ExpenseTracker.Rest.Dtos;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Rest.TemplateDtos;

namespace ExpenseTracker.Rest.MapperProfiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();
            
            CreateMap<ExpenseDto, Expense>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<Expense, ExpenseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            
            CreateMap<ExpenseTemplateDto, Expense>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Expense, ExpenseTemplateDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}