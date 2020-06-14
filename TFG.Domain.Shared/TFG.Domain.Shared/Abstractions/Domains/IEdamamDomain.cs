using System.Collections.Generic;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Shared.Abstractions.Domains
{
    public interface IEdamamDomain
    {
        List<EdamamRecipe> GetRecipeByName(string text);
    }
}
