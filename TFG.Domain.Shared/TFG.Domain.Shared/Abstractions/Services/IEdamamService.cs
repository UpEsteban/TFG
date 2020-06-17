using System.Collections.Generic;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Shared.Abstractions.Services
{
    public interface IEdamamService
    {
        List<EdamamRecipe> GetRecipeByName(string text);
    }
}
