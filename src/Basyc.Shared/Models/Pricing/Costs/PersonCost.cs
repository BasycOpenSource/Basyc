using System.Collections.ObjectModel;

namespace Basyc.Shared.Models.Pricing.Costs;

public record PersonCost(ReadOnlyCollection<ItemCost> Items, Cash TotalCost);
