using AutoMapper;
using System.Collections.Generic;
using System.Linq;
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

        public List<EdamamRecipe> GetRecipeByName(string text)
        {
            var model = repository.GetRecipeByName(text).hits.Select(x => x.recipe).Take(3).ToList();

            return Mappper(model);
        }

        private List<EdamamRecipe> Mappper(List<EdamamRecipeDTO.Recipe> model)
        {
            var result = new List<EdamamRecipe>();

            foreach (var r in model)
            {
                var ingredients = mapper.Map<List<EdamamRecipeDTO.Ingredient>, List<EdamamRecipe.Ingredient>>(r.ingredients.ToList());

                var digest = mapper.Map<List<EdamamRecipeDTO.Digest>, List<EdamamRecipe.Digest>>(r.digest.ToList());

                result.Add(new EdamamRecipe
                {
                    uri = r.uri,
                    label = r.label,
                    image = r.image,
                    source = r.source,
                    url = r.url,
                    shareAs = r.shareAs,
                    yield = r.yield,
                    dietLabels = r.dietLabels,
                    healthLabels = r.healthLabels,
                    cautions = r.cautions,
                    ingredientLines = r.ingredientLines,
                    ingredients = ingredients,
                    calories = r.calories,
                    totalWeight = r.totalWeight,
                    digest = digest,
                });
            }
            return result;
        }
    }
}
