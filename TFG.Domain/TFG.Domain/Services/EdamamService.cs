using System.Collections.Generic;
using TFG.Domain.Shared.Abstractions.Domains;
using TFG.Domain.Shared.Abstractions.Services;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Services
{
    public class EdamamService : IEdamamService
    {
        private readonly IEdamamDomain domain;

        public EdamamService(IEdamamDomain domain)
        {
            this.domain = domain;
        }

        public List<EdamamRecipe> GetRecipeByName(string text)
        {
            return domain.GetRecipeByName(text);
        }
    }
}
