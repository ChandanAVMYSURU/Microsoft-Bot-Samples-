using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SignupBot
{
    [LuisModel("9dd7ca64-cb54-4a8a-9eec-c04bd435c2ac", "c1c5093a4b384c1887b612d52a79c2f0")]
    [Serializable]
    public class EmailSignupDialog : LuisDialog<object>
{
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I didn't understand.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greet")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello! Welcome to the email sign-up bot. What would you like to do?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Signup")]
        public async Task SignUp(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Great! I just need a few pieces of information to get you signed up.");

            var form = new FormDialog<SignupForm>(
                new SignupForm(),
                SignupForm.BuildForm,
                FormOptions.PromptInStart,
                result.Entities);

            context.Call<SignupForm>(form, SignUpComplete);
        }

        private async Task SignUpComplete(IDialogContext context, IAwaitable<SignupForm> result)
        {
            SignupForm form = null;
            try
            {
                form = await result;
            }
            catch (OperationCanceledException)
            {
            }

            if (form == null)
            {
                await context.PostAsync("You canceled the form.");
            }
            else
            {
                // Here we could call our signup service to complete the sign-up

                var message = $"Thanks! We signed up {form.EmailAddress} in zip code {form.ZipCode}.";
                await context.PostAsync(message);
            }

            context.Wait(MessageReceived);
        }
    }

    [Serializable]
    public class SignupForm
    {

        private const string EmailRegExPattern = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        private const string ZipRegExPattern = @"^([0-9]{5})(?:[-\s]*([0-9]{4}))?$";
        public string EmailAddress { get; set; }
        public string ZipCode { get; set; }

        public static IForm<SignupForm> BuildForm()
        {
            return new FormBuilder<SignupForm>()
                .Field(nameof(EmailAddress),validate:EmailValidator)
                .Field(nameof(ZipCode),validate: ZipValidator)
                .Build();
        }

        private static ValidateAsyncDelegate<SignupForm> EmailValidator = async (state, response) =>
        {
            var result = new ValidateResult { IsValid = true, Value = response };
            var email = (response as string).Trim();
            if (!Regex.IsMatch(email, EmailRegExPattern))
            {
                result.Feedback = "Sorry, that doesn't look like a valid email address.";
                result.IsValid = false;
            }

            return await Task.FromResult(result);
        };
        private static ValidateAsyncDelegate<SignupForm> ZipValidator = async (state, response) =>
        {
            var result = new ValidateResult { IsValid = true, Value = response };
            var zip = (response as string).Trim();
            if (!Regex.IsMatch(zip, ZipRegExPattern))
            {
                result.Feedback = "Sorry, that is not a valid zip code. A zip code should be 5 digits.";
                result.IsValid = false;
            }

            return await Task.FromResult(result);
        };
    }
}