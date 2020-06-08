using TFG.ServiceContracts.Models;

namespace TFG.Domain.Shared.Abstractions.Domains
{
    public interface IEdamamDomain
    {
        EdamamRecipe GetRecipeByName(string text);
    }
}
