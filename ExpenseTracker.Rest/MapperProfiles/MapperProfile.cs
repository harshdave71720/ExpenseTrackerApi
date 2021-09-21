using System;
using AutoMapper;
using ExpenseTracker.Rest.Dtos;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Rest.MapperProfiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();
            CreateMap<ExpenseDto, Expense>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<Expense, ExpenseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}