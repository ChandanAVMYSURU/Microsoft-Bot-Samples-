using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SandwitchBot
{
    public enum SandwichOptions
    {
        BLT, BlackForestHam, BuffaloChicken, ChickenAndBaconRanchMelt, ColdCutCombo, MeatballMarinara,
        OvenRoastedChicken, RoastBeef,
        [Terms(@"rotis\w* style chicken", MaxPhrase = 3)]
        RotisserieStyleChicken, SpicyItalian, SteakAndCheese, SweetOnionTeriyaki, Tuna,
        TurkeyBreast, Veggie
    };
    public enum LengthOptions { SixInch, FootLong };
    public enum BreadOptions
    {
        // Use an image if generating cards
        // [Describe(Image = @"https://placeholdit.imgix.net/~text?txtsize=12&txt=Special&w=100&h=40&txttrack=0&txtclr=000&txtfont=bold")]
        NineGrainWheat,
        NineGrainHoneyOat,
        Italian,
        ItalianHerbsAndCheese,
        Flatbread
    };
    public enum CheeseOptions { American, MontereyCheddar, Pepperjack };
    public enum ToppingOptions
    {
        // This starts at 1 because 0 is the "no value" value
        [Terms("except", "but", "not", "no", "all", "everything")]
        Everything = 1,
        Avocado, BananaPeppers, Cucumbers, GreenBellPeppers, Jalapenos,
        Lettuce, Olives, Pickles, RedOnion, Spinach, Tomatoes
    };
    public enum SauceOptions
    {
        ChipotleSouthwest, HoneyMustard, LightMayonnaise, RegularMayonnaise,
        Mustard, Oil, Pepper, Ranch, SweetOnion, Vinegar
    };





    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")]
    [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your sandwich? {||}")]
    // [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your sandwich? {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
    public class SandwichOrder
    {
        [Prompt("What kind of {&} would you like? {||}")]
        public SandwichOptions? Sandwich;
        [Prompt("What size of sandwich do you want? {||}")]
        public LengthOptions? Length;
        public BreadOptions? Bread;
        // An optional annotation means that it is possible to not make a choice in the field.
        [Optional]
        public CheeseOptions? Cheese;
        [Optional]
        public List<ToppingOptions> Toppings { get; set; }
        [Optional]
        public List<SauceOptions> Sauces;
        [Optional]
        [Template(TemplateUsage.NoPreference, "None")]
        public string Specials;
        public string DeliveryAddress;
        [Pattern(@"(Undefined control sequence \d)?\s*\d{3}(-|\s*)\d{4}")]
        public string PhoneNumber;
        [Optional]
        [Template(TemplateUsage.StatusFormat, "{&}: {:t}", FieldCase = CaseNormalization.None)]
        public DateTime? DeliveryTime;
        [Numeric(1, 5)]
        [Optional]
        [Describe("your experience today")]
        public double? Rating;
        public static IForm<SandwichOrder> BuildForm()
        {
            OnCompletionAsyncDelegate<SandwichOrder> processOrder = async (context, state) =>
            {
                await context.PostAsync("We are currently processing your sandwich. We will message you the status.");
            };

            return new FormBuilder<SandwichOrder>()
                        .Message("Welcome to the sandwich order bot!")
                        .Field(nameof(Sandwich))
                        .Field(nameof(Length))
                        .Field(nameof(Bread))
                        .Field(nameof(Cheese))
                        .Field(nameof(Toppings))
                        .Field(nameof(SandwichOrder.Sauces))
                        .Field(nameof(SandwichOrder.DeliveryAddress))
                        .Field(nameof(SandwichOrder.DeliveryTime), "What time do you want your sandwich delivered? {||}")
                        .Confirm("Do you want to order your {Length} {Sandwich} on {Bread} {&Bread} with {[{Cheese} {Toppings} {Sauces}]} to be sent to {DeliveryAddress} {?at {DeliveryTime:t}}?")
                        .AddRemainingFields()
                        .Message("Thanks for ordering a sandwich!")
                        .OnCompletion(processOrder)
                        .Build();
        }

        private static ValidateResult GetValidationResult(IEnumerable<ToppingOptions> values)
        {
            var result = new ValidateResult { IsValid = true, Value = values };
            if (values != null && values.Contains(ToppingOptions.Everything))
            {
                result.Value = (from ToppingOptions topping in Enum.GetValues(typeof(ToppingOptions))
                                where topping != ToppingOptions.Everything && !values.Contains(topping)
                                select topping).ToList();
            }
            return result;
        }
    }
}