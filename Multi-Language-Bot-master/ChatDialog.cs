using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace HotelBot
{
    [Serializable]
    [LuisModel("9dd7ca64-cb54-4a8a-9eec-c04bd435c2ac", "c1c5093a4b384c1887b612d52a79c2f0")]
    public class ChatDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Default;

            await context.PostAsync(response.ToUserLocale(context));

            context.Wait(MessageReceived);
        }

        [LuisIntent("Greet")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Greeting;

            await context.PostAsync(response.ToUserLocale(context));

            context.Wait(MessageReceived);
        }

        //[LuisIntent("Farewell")]
        //public async Task Farewell(IDialogContext context, LuisResult result)
        //{
        //    var response = ChatResponse.Farewell;

        //    await context.PostAsync(response.ToUserLocale(context));

        //    context.Wait(MessageReceived);
        //}

        //[LuisIntent("SwimmingPool")]
        //public async Task SwimmingPool(IDialogContext context, LuisResult result)
        //{
        //    var response = ChatResponse.SwimmingPool;

        //    await context.PostAsync(response.ToUserLocale(context));

        //    context.Wait(MessageReceived);
        //}

        //[LuisIntent("Location")]
        //public async Task Location(IDialogContext context, LuisResult result)
        //{
        //    var response = ChatResponse.Location;

        //    await context.PostAsync(response.ToUserLocale(context));

        //    context.Wait(MessageReceived);
        //}

        //[LuisIntent("Restaurant")]
        //public async Task Restaurant(IDialogContext context, LuisResult result)
        //{
        //    var response = ChatResponse.Restaurant;

        //    await context.PostAsync(response.ToUserLocale(context));

        //    context.Wait(MessageReceived);
        //}


        //[LuisIntent("Wifi")]
        //public async Task Wifi(IDialogContext context, LuisResult result)
        //{
        //    var response = ChatResponse.Wifi;

        //    await context.PostAsync(response.ToUserLocale(context));

        //    context.Wait(MessageReceived);
        //}
    }

    
}