using TFG.ServiceContracts.Models;

namespace TFG.Domain.Shared.Abstractions.Services
{
    public interface IEdamamService
    {
        EdamamRecipe GetRecipeByName(string text);
    }
}
