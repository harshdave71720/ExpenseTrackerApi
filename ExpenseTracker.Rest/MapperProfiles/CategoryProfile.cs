using System;
using AutoMapper;
using ExpenseTracker.Rest.Dtos;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Rest.MapperProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();
        }
    }
}