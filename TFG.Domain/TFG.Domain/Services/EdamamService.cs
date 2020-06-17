using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            var result = domain.GetRecipeByName(text);

            foreach (var item in result)
            {
                var labels = AllergyEdamamMappers(item.healthLabels.ToList());

                labels.AddRange(item.healthLabels.ToList());

                item.healthLabels = labels.ToArray();
            }

            return result;
        }

        public static List<string> AllergyEdamamMappers(List<string> allergyName)
        {
            List<string> result = new List<string>();

            if (allergyName.Where(x => x.ToLower().Equals("gluten-free")).FirstOrDefault() == null)
            {
                result.Add("Gluten_Emoji");
            }

            if (allergyName.Where(x => x.ToLower().Equals("peanut-free")).FirstOrDefault() == null)
            {
                result.Add("Cacahuete_Emoji");
            }

            if (allergyName.Where(x => x.ToLower().Equals("shellfish-free")).FirstOrDefault() == null)
            {
                result.Add("Crustaceos_Emoji");
            }

            if (allergyName.Where(x => x.ToLower().Equals("egg-free")).FirstOrDefault() == null)
            {
                result.Add("Huevo_Emoji");
            }

            if (allergyName.Where(x => x.ToLower().Equals("dairy-free")).FirstOrDefault() == null)
            {
                result.Add("Leche_Emoji");
            }

            if (allergyName.Where(x => x.ToLower().Equals("fish-free")).FirstOrDefault() == null)
            {
                result.Add("Pescado_Emoji");
            }

            return result;
        }

        public static string AllergyMappers(string allergyName)
        {
            var result = string.Empty;

            switch (allergyName.ToLower())
            {
                case "gluten":
                    result = "Gluten-Free";
                    break;
                case "cacahuete":
                    result = "peanut-free";
                    break;
                case "crustaceos":
                    result = "crustacean-free";
                    break;
                case "huevo":
                    result = "egg-free";
                    break;
                case "leche":
                    result = "dairy-free";
                    break;
                case "pescado":
                    result = "fish-free";
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
