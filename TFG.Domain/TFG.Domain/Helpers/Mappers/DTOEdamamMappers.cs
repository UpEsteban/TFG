using AutoMapper;
using TFG.Domain.Shared.Models;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Helpers.Mappers
{
    public class DTOEdamamMappers : Profile
    {
        public DTOEdamamMappers()
        {
            CreateMap<EdamamRecipeDTO, EdamamRecipe>();
            CreateMap<EdamamRecipe, EdamamRecipeDTO>();
        }
    }
}
