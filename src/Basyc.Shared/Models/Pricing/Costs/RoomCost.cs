using Basyc.Shared.Models.Pricing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Basyc.Shared.Models.Pricing.Costs;

public record RoomCost(ReadOnlyCollection<PersonCost> PersonCosts, ReadOnlyCollection<ItemCost> RoomItems, Cash TotalCost);
