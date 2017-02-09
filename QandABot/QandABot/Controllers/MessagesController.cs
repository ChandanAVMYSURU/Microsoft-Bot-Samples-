using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace QandABot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new FAQDialog());
            }
            else
            {
                //add code to handle errors, or non-messaging activities

            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }



        //Inherit from the QnAMakerDialog
        [Serializable]
        public class BasicQnAMakerDialog : QnAMakerDialog
        {
            //Parameters to QnAMakerService are:
            //Compulsory: subscriptionKey, knowledgebaseId, 
            //Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
            public BasicQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute("4f9121eb86204e3d8f1f0f2adfb334e1", "99938c2e-b5c2-486c-b329-577ec2d10183", "No good match in FAQ.", 0.5)))
            {
            }
        }

        /// <summary>
        /// Simple Dialog, that invokes the QnAMaker if the incoming message is a question
        /// </summary>
        [Serializable]
        public class FAQDialog : IDialog<object>
        {
            public async Task StartAsync(IDialogContext context)
            {
                context.Wait(MessageReceivedAsync);
            }

            public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var message = await argument;

                //Call the QnAMaker Dialog if the message is a question.
                if (IsQuestion(message.Text))
                {
                    await context.Forward(new BasicQnAMakerDialog(), AfterQnA, message, CancellationToken.None);
                }
                else
                    await context.PostAsync("This doesn't look like a question.");

                context.Wait(MessageReceivedAsync);
            }

            //Callback, after the QnAMaker Dialog returns a result.
            public async Task AfterQnA(IDialogContext context, IAwaitable<object> argument)
            {
                context.Wait(MessageReceivedAsync);
            }

            //Simple check if the message is a potential question.
            private bool IsQuestion(string message)
            {
                //List of common question words
                List<string> questionWords = new List<string>() { "who", "what", "why", "how", "when" };

                //Question word present in the message
                Regex questionPattern = new Regex(@"\b(" + string.Join("|", questionWords.Select(Regex.Escape).ToArray()) + @"\b)", RegexOptions.IgnoreCase);

                //Return true if a question word present, or the message ends with "?"
                if (questionPattern.IsMatch(message) || message.EndsWith("?"))
                    return true;
                else
                    return false;
            }
        }
    }
