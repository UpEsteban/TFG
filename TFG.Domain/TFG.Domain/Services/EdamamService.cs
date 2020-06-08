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

        public EdamamRecipe GetRecipeByName(string text)
        {
            var res = domain.GetRecipeByName(text);
            return res;
        }
    }
}
