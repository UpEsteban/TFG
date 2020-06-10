using Microsoft.Bot.Schema;
using System.Collections.Generic;
using TFG.Bot.Resources;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Bot.Cards
{
    public static class HeroCards
    {
        private static readonly string url = BlobStorage.baseUrl;

        public static Activity WelcomeProfile(Activity activity, IMessagesService messagesService)
        {
            var reply = Welcome(activity, messagesService);

            var attachments = new List<Attachment>();

            string profile = messagesService.Get(MessagesKey.Key.Profile.ToString()).Value;

            var Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, value: profile, title: profile) };

            var Images = new List<CardImage> { new CardImage { Alt = url + "welcome_login.jpg", Url = url + "welcome_login.jpg" } };

            attachments.Add(GetHeroCard(profile, string.Empty, string.Empty, null, Buttons, Images));

            attachments.AddRange(reply.Attachments);

            reply.Attachments = attachments;

            return reply;
        }

        public static Activity WelcomeLogin(Activity activity, IMessagesService messagesService)
        {
            var reply = Welcome(activity, messagesService);

            var attachments = new List<Attachment>();

            string login = messagesService.Get(MessagesKey.Key.Login.ToString()).Value;

            var Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, value: login, title: login) };

            var Images = new List<CardImage> { new CardImage { Alt = url + "welcome_login.jpg", Url = url + "welcome_login.jpg" } };

            attachments.Add(GetHeroCard(login, string.Empty, string.Empty, null, Buttons, Images));

            attachments.AddRange(reply.Attachments);

            reply.Attachments = attachments;

            return reply;
        }

        public static Activity Welcome(Activity activity, IMessagesService messagesService)
        {
            var attachments = new List<Attachment>();

            var reply = activity.CreateReply();

            #region RecipeSearch

            string recipeSearchCB = messagesService.Get(MessagesKey.Key.RecipeSearchCB.ToString()).Value;
            string recipeSearch = messagesService.Get(MessagesKey.Key.RecipeSearch.ToString()).Value;

            var Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, value: recipeSearchCB, title: recipeSearch) };
            var Images = new List<CardImage> { new CardImage { Alt = url + "welcome_recipe.jpg", Url = url + "welcome_recipe.jpg" } };

            attachments.Add(GetHeroCard(recipeSearch, string.Empty, string.Empty, null, Buttons, Images));
            #endregion

            #region Recipe Allergy Search

            string recipeAllergySearchCB = messagesService.Get(MessagesKey.Key.RecipeAllergySearchCB.ToString()).Value;
            string recipeAllergySearch = messagesService.Get(MessagesKey.Key.RecipeAllergySearch.ToString()).Value;

            Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, value: recipeAllergySearchCB, title: recipeAllergySearch) };
            Images = new List<CardImage> { new CardImage { Alt = url + "welcome_allergy.jpeg", Url = url + "welcome_allergy.jpeg" } };

            attachments.Add(GetHeroCard(recipeAllergySearch, string.Empty, string.Empty, null, Buttons, Images));
            #endregion

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = attachments;
            reply.Text = messagesService.Get(MessagesKey.Key.WelcomeExtend.ToString()).Value;

            return reply;
        }

        private static Attachment GetHeroCard(string Tittle, string Text, string Subtittle, CardAction Tap, List<CardAction> Buttons, List<CardImage> Images)
        {
            return new HeroCard
            {
                Title = Tittle,
                Text = Text,
                Subtitle = Subtittle,
                Tap = Tap,
                Buttons = Buttons,
                Images = Images
            }.ToAttachment();
        }
    }
}
