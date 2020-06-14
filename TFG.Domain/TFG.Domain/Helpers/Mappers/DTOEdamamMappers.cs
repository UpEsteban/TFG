using AutoMapper;
using TFG.Domain.Shared.Models;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Helpers.Mappers
{
    public class DTOEdamamMappers : Profile
    {
        public DTOEdamamMappers()
        {
            CreateMap<EdamamRecipeDTO.Recipe, EdamamRecipe>();
            CreateMap<EdamamRecipeDTO.Ingredient, EdamamRecipe.Ingredient>();
            CreateMap<EdamamRecipeDTO.Digest, EdamamRecipe.Digest>();
            CreateMap<EdamamRecipeDTO.Sub, EdamamRecipe.Sub>();

            CreateMap<EdamamRecipe, EdamamRecipeDTO.Recipe>();
            CreateMap<EdamamRecipe.Ingredient, EdamamRecipeDTO.Ingredient>();
            CreateMap<EdamamRecipe.Digest, EdamamRecipeDTO.Digest>();
            CreateMap<EdamamRecipe.Sub, EdamamRecipeDTO.Sub>();

            CreateMap<EdamamRecipeDTO, EdamamRecipe>();
            CreateMap<EdamamRecipe, EdamamRecipeDTO>();
        }
    }
}
