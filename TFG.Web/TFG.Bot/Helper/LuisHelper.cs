using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System.Collections.Generic;
using System.Linq;

namespace TFG.Bot.Helper
{
    public static class LuisHelper
    {
        public static string GetNormalizedValueFromEntity(EntityModel entity)
        {
            var jsonLuisType = entity.AdditionalProperties.Values.First().ToString();
            var luisTypeEntity = Serializer.JsonToObject<LuisNormalizedValue>(jsonLuisType);
            var objectString = luisTypeEntity.values.First();
            return objectString;
        }
    }

    public class LuisNormalizedValue
    {
        public List<string> values { get; set; }
    }
}
