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

namespace NewsBot
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
                reply = activity.CreateReply("Error occured");
                await connector.Conversations.ReplyToActivityAsync(reply);
                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
                //add code to handle errors, or non-messaging activities
            }


            const string apiKey = "5215eb88c8b0431ebd4b523d0c7b7d26";
            string queryUri = "https://api.cognitive.microsoft.com/bing/v5.0/news/search";

            //Helper objects to call the News Search API and store the response
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey); //authentication header to pass the API key
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            string bingRawResponse; //raw response from REST endpoint
            BingTrendingNewsResults bingJsonResponse = null; //Deserialized response 

            try
            {
                bingRawResponse = await httpClient.GetStringAsync(queryUri);
                bingJsonResponse = JsonConvert.DeserializeObject<BingTrendingNewsResults>(bingRawResponse);
            }
            catch (Exception e)
            {
                //add code to handle exceptions while calling the REST endpoint and/or deserializing the object
            }

            NewsResult[] newsResult = bingJsonResponse.value;

            if (newsResult == null || newsResult.Length == 0)
            {
                //add code to handle the case where results are null are zero
            }

            int newsResultCount = Math.Min(5, newsResult.Length); // show up to 5 trending news 
            Activity replyMessage = activity.CreateReply("Here are the top trending news I found:");
            replyMessage.Recipient = activity.From;
            replyMessage.Type = ActivityTypes.Message;
            replyMessage.AttachmentLayout = "carousel";
            replyMessage.Attachments = new List<Attachment>();

            for (int i = 0; i < newsResultCount; i++)
            {
                Attachment attachment = new Attachment();
                attachment.ContentType = "application/vnd.microsoft.card.hero";

                //Construct Card
                HeroCard card = new HeroCard();
                card.Title = newsResult[i].name;
                card.Subtitle = newsResult[i].description;

                //Add Card Image
                card.Images = new List<CardImage>();
                CardImage img = new CardImage();
                img.Url = newsResult[i].image.thumbnail.contentUrl;
                card.Images.Add(img);

                //Add Card Buttons
                card.Buttons = new List<CardAction>();
                CardAction btnArticle = new CardAction();

                //Go to article button
                btnArticle.Title = "Go to article";
                btnArticle.Type = "openUrl";
                btnArticle.Value = newsResult[i].url;
                card.Buttons.Add(btnArticle);
                attachment.Content = card;

                replyMessage.Attachments.Add(attachment);
            }
            //Reply to user message with the news carousel attachment
            await connector.Conversations.ReplyToActivityAsync(replyMessage);
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
}