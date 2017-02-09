using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace SearchBot
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

            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            Activity reply;
            if (activity == null || activity.GetActivityType() != ActivityTypes.Message)
            {
                //add code to handle errors, or non-messaging activities

                reply = activity.CreateReply("Error occured");
                await connector.Conversations.ReplyToActivityAsync(reply);
                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }

            const string apiKey = "5215eb88c8b0431ebd4b523d0c7b7d26";
            string queryUri = "https://api.cognitive.microsoft.com/bing/v5.0/images/search"
                              + "?q=" + activity.Text
                              + "&imageType=AnimatedGif"; //parameter to filter by GIF image type

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey); //authentication header to pass the API key
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            string bingRawResponse = null;
            BingImageSearchResponse bingJsonResponse = null;

            try
            {
                bingRawResponse = await client.GetStringAsync(queryUri);
                bingJsonResponse = JsonConvert.DeserializeObject<BingImageSearchResponse>(bingRawResponse);
            }
            catch (Exception e)
            {


                reply = activity.CreateReply("Error occured");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            ImageResult[] imageResult = bingJsonResponse.value;
            if (imageResult == null || imageResult.Length == 0)
            {
                //add code to handle the case where results are null or zero
            }
            string firstResult = imageResult[0].contentUrl;

            var replyMessage = activity.CreateReply();
            replyMessage.Recipient = activity.From;
            replyMessage.Type = ActivityTypes.Message;
            replyMessage.Text = $"Here is what i found:";
            replyMessage.Attachments = new System.Collections.Generic.List<Attachment>();
            replyMessage.Attachments.Add(new Attachment()
            {
                ContentUrl = firstResult,
                ContentType = "image/png"
            });

            //Reply to user message with image attachment
            await connector.Conversations.ReplyToActivityAsync(replyMessage);
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);

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
}