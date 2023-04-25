using Basyc.Shared.Models.Pricing;
using Basyc.Shared.Models.Pricing.Costs;
using Bogus;

namespace Basyc.Shared.Tests.FakeData;

public static class CostFakeData
{
    public static List<AccommodationCost> GetAccommodationCosts(int count, Currencies? sharedCurrency = null)
    {
        sharedCurrency ??= new Faker().Random.Enum<Currencies>();

        if (count == 0)
        {
            return new List<AccommodationCost>();
        }

        var costs = new Faker<AccommodationCost>()
            .StrictMode(true)
            .CustomInstantiator(x => new AccommodationCost(
                default!,
                default!,
                default!))
            .RuleFor(x => x.AccomodationItems, (x, a) => GetItemCosts(x.Random.Number(0, 5), true, sharedCurrency).AsReadOnly())
            .RuleFor(x => x.Rooms, x => GetRoomCosts(x.Random.Int(0, 5), sharedCurrency).AsReadOnly())
            .RuleFor(x => x.TotalCost, (x, r) =>
            {
                return new Cash(sharedCurrency.Value, r.AccomodationItems.Sum(x => x.CostPerOne.Amount) + r.Rooms.Sum(x => x.TotalCost.Amount));
            })
            .Generate(count);

        return costs;
    }

    public static List<ItemCost> GetItemCosts(int count, bool allCurrenciesSame = true, Currencies? sharedCurrency = null)
    {
        sharedCurrency ??= new Faker().Random.Enum<Currencies>();

        if (count == 0)
        {
            return new List<ItemCost>();
        }

        var costs = new Faker<ItemCost>()
            .StrictMode(true)
            .CustomInstantiator(x =>
                new ItemCost(x.Random.String(), new Dictionary<string, string>(), new Cash(Currencies.Eur, x.Random.Decimal()), default, default!))
            .RuleFor(x => x.Name, x => x.Random.Words(x.Random.Int(1, 5)))
            .RuleFor(x => x.CostPerOne, x => new Cash(sharedCurrency.Value, x.Random.Decimal(0, 1000)))
            .RuleFor(x => x.Descriptions, x => new Dictionary<string, string>(new Faker<Tuple<string, string>>()
                .CustomInstantiator(x => new Tuple<string, string>(x.IndexGlobal.ToString(), string.Empty))
                //.RuleFor(x => x.Item1, (Faker x) => x.Random.Word()) ///ffffgdfguifguguuguduflkad ou  iojqwd jioiiojasdadasdsdtrioiiiasdoij iiojasdasddddasdasdasdasd
                .RuleFor(x => x.Item2, x => x.Random.Words(x.Random.Int(0, 5)))
                .Generate(x.Random.Int(0, 10)).Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2))))
            .RuleFor(x => x.Count, x => x.Random.Int(0, 5))
            .FinishWith((x, a) =>
            {
                sharedCurrency = allCurrenciesSame ? sharedCurrency : new Faker().Random.Enum<Currencies>();
            })
            .Generate(count);

        return costs;
    }

    public static List<RoomCost> GetRoomCosts(int count, Currencies? sharedCurrency = null)
    {
        sharedCurrency ??= new Faker().Random.Enum<Currencies>();
        if (count == 0)
        {
            return new List<RoomCost>();
        }

        var rooms = new Faker<RoomCost>()
            .StrictMode(true)
            .CustomInstantiator(x => new RoomCost(null!, null!, null!))
            .RuleFor(x => x.PersonCosts, x => GetPersonCosts(x.Random.Int(0, 5)).AsReadOnly())
            .RuleFor(x => x.RoomItems, x => GetItemCosts(x.Random.Int(0, 5), true, sharedCurrency).AsReadOnly())
            .RuleFor(x => x.TotalCost, (x, r) =>
            {
                var cur = r.RoomItems.Count == 0 ? x.Random.Enum<Currencies>() : r.RoomItems[0].CostPerOne.Currency;
                return new Cash(cur, r.RoomItems.Sum(x => x.CostPerOne.Amount));
            })
            .Generate(count);

        return rooms;
    }

    public static List<PersonCost> GetPersonCosts(int count)
    {
        if (count == 0)
        {
            return new List<PersonCost>();
        }

        var people = new Faker<PersonCost>()
            .StrictMode(true)
            .CustomInstantiator(x => new PersonCost(null!, null!))
            .RuleFor(x => x.Items, x => GetItemCosts(x.Random.Int(0, 5)).AsReadOnly())
            .RuleFor(x => x.TotalCost, (x, r) =>
            {
                var cur = r.Items.Count == 0 ? x.Random.Enum<Currencies>() : r.Items[0].CostPerOne.Currency;
                return new Cash(cur, r.Items.Sum(x => x.CostPerOne.Amount));
            })
            .Generate(count);
        return people;
    }
}
