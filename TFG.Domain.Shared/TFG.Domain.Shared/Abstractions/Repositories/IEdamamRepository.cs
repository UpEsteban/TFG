using TFG.Domain.Shared.Models;

namespace TFG.Domain.Shared.Abstractions.Repositories
{
    public interface IEdamamRepository
    {
        EdamamRecipeDTO GetRecipeByName(string text);
    }
}
