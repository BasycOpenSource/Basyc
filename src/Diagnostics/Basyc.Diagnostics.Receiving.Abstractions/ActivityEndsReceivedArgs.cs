using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Receiving.Abstractions;

#pragma warning disable CA1819 // Properties should not return arrays
public record ActivityEndsReceivedArgs(ActivityEnd[] ActivityEnds);
