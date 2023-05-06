using System.Collections.ObjectModel;

namespace Basyc.Shared.Models.Pricing.Costs;

public record AccommodationCost(
    ReadOnlyCollection<RoomCost> Rooms,
    ReadOnlyCollection<ItemCost> AccomodationItems,
    Cash TotalCost);
