using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using TFG.ServiceContracts.Models;

namespace TFG
{
    public class ConversationData
    {
        public RecognizerResult RecognizerResult { get; set; }

        public LuisResult LuisResult { get; set; }

        public User User { get; set; }

        public string LastRecipeSearch { get; set; }
    }
}
