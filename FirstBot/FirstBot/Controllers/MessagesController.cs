using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using WebApi;

namespace FirstBot
{
    //Dialog basics
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public HttpClient client = new HttpClient();

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new RestwithLuis());
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        [Serializable]
        public class CaseDialog : IDialog<object>
        {
            protected int number1 { get; set; }
            protected int updateid { get; set; }
            public CaseDialog(HttpClient client) {

                client.BaseAddress = new Uri("http://localhost:6838/");

            }
            public async Task StartAsync(IDialogContext context)
            {
                context.Wait(MessageReceivedStart);
            }
            public async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                await context.PostAsync("Enter Something");
                
                context.Wait(MessageReceivedOperationChoice);
            }

            public async Task MessageReceivedOperationChoice(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {

                var message = await argument;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:6838/");

                if (message.Text.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync("Enter case id");
                    context.Wait(MessageReceivedCaseId);
                }
                else if (message.Text.Equals("name", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync("Enter case name:");
                    context.Wait(MessageReceivedCaseName);
                }
                else if (message.Text.Equals("all", StringComparison.InvariantCultureIgnoreCase))
                {
                    HttpResponseMessage response = await client.GetAsync("/api/Case");
                    response.EnsureSuccessStatusCode();
                    var cases = await response.Content.ReadAsAsync<IEnumerable<Case>>();
                    List<Case> listOfCases = cases.ToList();
                    int c = 0;
                    foreach (Case i in listOfCases) {
                        await context.PostAsync("Case Id : "+i.id+" name :"+i.name+" info: "+i.info);
                    }
                    context.Wait(ChoiceToAddCase);

                }
                else if (message.Text.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync("Enter case id:");
                    context.Wait(MessageReceivedCaseIdToDelete);
                }
                else if (message.Text.Equals("update",StringComparison.InvariantCultureIgnoreCase)) {

                    HttpResponseMessage response = await client.GetAsync("/api/Case");
                    response.EnsureSuccessStatusCode();
                    var cases = await response.Content.ReadAsAsync<IEnumerable<Case>>();
                    List<Case> listOfCases = cases.ToList();
                    string ids="";
                    foreach (Case i in listOfCases)
                    {
                        ids += i.id.ToString();
                    }
                    await context.PostAsync("Enter the case id you want to update  your id should be one of this " + ids);
                    context.Wait(RecieveUpdateId);
                }
                else
                {

                    context.Wait(MessageReceivedStart);
                }
            }
            public async Task RecieveUpdateId(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var uid = await argument;
                this.updateid = int.Parse(uid.Text);
                await context.PostAsync("Enter the case details in this format: name,info ");
                context.Wait(MessageReceivedUpdateCase);

            }

            public async Task MessageReceivedUpdateCase(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var updatedetails = await argument;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:6838/");
                string[] array = updatedetails.Text.Split(',');
                Case obj = new Case();
                obj.id = this.updateid;
                obj.name = array[0];
                obj.info = array[1];
                var response = await client.PutAsJsonAsync("/api/Case/", obj);
                response.EnsureSuccessStatusCode();
                await context.PostAsync("Successfully updated");
                context.Wait(MessageReceivedStart);

            }

            public async Task ChoiceToAddCase(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                await context.PostAsync("Want to add new case ? Yes | No  ");
                context.Wait(MessageReceivedAddCase);
               
            }
            public async Task MessageReceivedCaseIdToDelete(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var idtext = await argument;
                int id = int.Parse(idtext.Text);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:6838/");

                HttpResponseMessage response = await client.DeleteAsync("/api/Case/"+id);
                response.EnsureSuccessStatusCode();
                await context.PostAsync("Successfully deleted");

                context.Wait(MessageReceivedStart);
            }
            

            public async Task MessageReceivedAddCase(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var choice = await argument;
                if (choice.Text.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync("Enter the case details like id,name,info");
                    context.Wait(RecieveCaseInfo);
                }
                else
                {
                    context.Wait(MessageReceivedStart);
                }
            }

            public async Task RecieveCaseInfo(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var choice = await argument;
                string c = choice.Text;
                string[] caseinfo = c.Split(',');
                Case newObj = new Case();
                newObj.id = int.Parse(caseinfo[0]);
                newObj.name = caseinfo[1];
                newObj.info = caseinfo[2];

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:6838/");

                var response = await client.PostAsJsonAsync("/api/Case/",newObj);
                response.EnsureSuccessStatusCode();
                await context.PostAsync("Successfully added ");   
                context.Wait(MessageReceivedStart);
            }




            public async Task MessageReceivedCaseId(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var numbers = await argument;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:6838/");
                int id = int.Parse(numbers.Text);
                HttpResponseMessage response = await client.GetAsync("/api/Case/"+id);
                response.EnsureSuccessStatusCode();
                var cases = await response.Content.ReadAsAsync<Case>();
                Case caseObj = cases;


                //  var number2 = int.Parse(numbers.Text);
                await context.PostAsync("Case Id : "+caseObj.id +"name : "+caseObj.name +"info : "+caseObj.info);

                context.Wait(MessageReceivedStart);
            }

            public async Task MessageReceivedCaseName(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var number = await argument;
                //var num = double.Parse(number.Text);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:6838/");
                string name = number.Text;
                HttpResponseMessage response = await client.GetAsync("/api/Case?name=" +name);
                response.EnsureSuccessStatusCode();
                var cases = await response.Content.ReadAsAsync<Case>();
                Case caseObj = cases;

                await context.PostAsync("Case Id : " + caseObj.id + "name : " + caseObj.name + "info : " + caseObj.info);

                context.Wait(MessageReceivedStart);
            }

         
        }

    }
}