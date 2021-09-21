using AutoMapper;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Web.ViewModels;

namespace ExpenseTracker.Web.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ExpenseViewModel, Expense>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<Expense, ExpenseViewModel>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
                //.ForMember(dest => dest.Category, opt => opt.MapFrom(src => new Category(src.Category, null)));
            
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();
        }
    }
}