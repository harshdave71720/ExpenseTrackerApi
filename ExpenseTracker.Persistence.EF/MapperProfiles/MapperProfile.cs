using AutoMapper;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Persistence.Entities;

namespace ExpenseTracker.Persistence
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Expense, ExpenseEntity>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Id))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .MaxDepth(1);
            CreateMap<ExpenseEntity, Expense>().MaxDepth(1);
            CreateMap<Category, CategoryEntity>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .MaxDepth(1);
            CreateMap<CategoryEntity, Category>().MaxDepth(1);
            CreateMap<UserEntity, User>();
        }
    }
}