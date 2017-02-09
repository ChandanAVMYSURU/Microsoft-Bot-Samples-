using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WebApi;

namespace FirstBot
{
    [LuisModel("269c348a-91e3-4506-bbac-c7a8ba7682f8", "c1c5093a4b384c1887b612d52a79c2f0")]
    [Serializable]
    public class RestwithLuis : LuisDialog<object>
    {
        [LuisIntent("GetAll")]
        public async Task GetAll(IDialogContext context, LuisResult result)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:6838/");
            HttpResponseMessage response = await client.GetAsync("/api/Case");
            response.EnsureSuccessStatusCode();
            var cases = await response.Content.ReadAsAsync<IEnumerable<Case>>();
            List<Case> listOfCases = cases.ToList();
            int c = 0;
            foreach (Case i in listOfCases)
            {
                await context.PostAsync("Case Id : " + i.id + " name :" + i.name + " info: " + i.info);
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetById")]

        public async Task GetById(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Enter the case Id");
            context.Wait(MessageReceivedCaseId);
        }


        public async Task MessageReceivedCaseId(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var numbers = await argument;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:6838/");
            int id = int.Parse(numbers.Text);
            HttpResponseMessage response = await client.GetAsync("/api/Case/" + id);
            response.EnsureSuccessStatusCode();
            var cases = await response.Content.ReadAsAsync<Case>();
            Case caseObj = cases;
            await context.PostAsync("Case Id : " + caseObj.id + "name : " + caseObj.name + "info : " + caseObj.info);
            context.Wait(MessageReceived);
        }


        [LuisIntent("ADD")]

        public async Task AddCase(IDialogContext context, LuisResult result)
        {
           await context.PostAsync("Enter the case details like id,name,info");

            context.Wait(RecieveCaseInfo);

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

            var response = await client.PostAsJsonAsync("/api/Case/", newObj);
            response.EnsureSuccessStatusCode();
            await context.PostAsync("Successfully added ");
            context.Wait(MessageReceived);
        }


        [LuisIntent("Update")]

        public async Task Update(IDialogContext context, LuisResult result)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:6838/");

            HttpResponseMessage response = await client.GetAsync("/api/Case");
            response.EnsureSuccessStatusCode();
            var cases = await response.Content.ReadAsAsync<IEnumerable<Case>>();
            List<Case> listOfCases = cases.ToList();
            string ids = "";
            foreach (Case i in listOfCases)
            {
                ids += i.id.ToString() +" , ";
            }
            await context.PostAsync("Enter the case id you want to update  your id should be one of this " + ids);
            context.Wait(RecieveUpdateId);

        }
        public int updateid;

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
            context.Wait(MessageReceived);

        }

        [LuisIntent("Delete")]
        public async Task delete(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Enter case id:");
            context.Wait(MessageReceivedCaseIdToDelete);
        }
        public async Task MessageReceivedCaseIdToDelete(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var idtext = await argument;
            int id = int.Parse(idtext.Text);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:6838/");

            HttpResponseMessage response = await client.DeleteAsync("/api/Case/" + id);
            response.EnsureSuccessStatusCode();
            await context.PostAsync("Successfully deleted");

            context.Wait(MessageReceived);
        }
    }
}

