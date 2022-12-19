//using Bogus;
//using Kontrer.Shared.Models;
//using Kontrer.Shared.Models.Pricing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Kontrer.Shared.Tests.FakeData
//{
//    public static class AccommodationFakeData
//    {
//        public static List<FinishedAccommodationModel> GetAccommodationsWithoutCustomers(int count)
//        {
//            if (count == 0)
//                return new List<FinishedAccommodationModel>();

//            List<FinishedAccommodationModel> accommodations = GetAccommodationsWithoutCustomer(count);
//            return accommodations;
//        }

//        public static List<FinishedAccommodationModel> GetAccommodationsWithoutCustomer(int count, bool hasSharedCurrencies = true, Currencies? sharedCurrency = null)
//        {
//            sharedCurrency ??= new Faker().Random.Enum<Currencies>();

//            if (count == 0)
//                return new List<FinishedAccommodationModel>();

//            int idCounter = 0;
//            var accommodations = new Faker<FinishedAccommodationModel>()
//                .StrictMode(true)
//                .RuleFor(x => x.Blueprint, (Faker x) => BlueprintFakeData.GetAccommodationBlueprints(1, true, sharedCurrency)[0])
//                .RuleFor(x => x.AccommodationId, x => idCounter++)
//                .RuleFor(x => x.Blueprint.CustomerId, x => null)
//                .RuleFor(x => x.Cost, (Faker x, FinishedAccommodationModel a) => CostFakeData.GetAccommodationCosts(1, sharedCurrency)[0])
//                .RuleFor(x => x.OwnersPrivateNotes, x => x.Random.Words(x.Random.Int(0, 5)))
//                .FinishWith((x, a) =>
//                {
//                    sharedCurrency = hasSharedCurrencies ? sharedCurrency : x.Random.Enum<Currencies>();
//                })
//                .Generate(count);

//            return accommodations;
//        }

//    }
//}