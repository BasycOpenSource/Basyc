using System.Collections.ObjectModel;

namespace Basyc.Shared.Models.Pricing.Costs;

public record RoomCost(ReadOnlyCollection<PersonCost> PersonCosts, ReadOnlyCollection<ItemCost> RoomItems, Cash TotalCost);
