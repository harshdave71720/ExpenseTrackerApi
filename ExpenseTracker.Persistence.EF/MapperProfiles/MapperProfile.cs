using AutoMapper;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Persistence.Entities;

namespace ExpenseTracker.Persistence
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Expense, ExpenseEntity>();
            CreateMap<ExpenseEntity, Expense>();
        }
    }
}