namespace Basyc.Shared.Models.Pricing.Costs;

public record ItemCost(string Name, Dictionary<string, string> Descriptions, Cash CostPerOne, int Count, Cash TotalCost);
