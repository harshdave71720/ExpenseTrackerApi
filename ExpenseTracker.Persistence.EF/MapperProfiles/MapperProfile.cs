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
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .MaxDepth(1);
            CreateMap<ExpenseEntity, Expense>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .MaxDepth(1);
            CreateMap<Category, CategoryEntity>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .MaxDepth(1);
            CreateMap<CategoryEntity, Category>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .MaxDepth(1);
            CreateMap<UserEntity, User>();
        }
    }
}