using AutoMapper;
using TFG.Domain.Shared.Abstractions.Domains;
using TFG.Domain.Shared.Abstractions.Repositories;
using TFG.Domain.Shared.Models;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Domains
{
    public class EdamamDomain : IEdamamDomain
    {
        private readonly IEdamamRepository repository;
        private readonly IMapper mapper;

        public EdamamDomain(IEdamamRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public EdamamRecipe GetRecipeByName(string text)
        {
            var model = repository.GetRecipeByName(text);

            return mapper.Map<EdamamRecipeDTO, EdamamRecipe>(model);
        }
    }
}
