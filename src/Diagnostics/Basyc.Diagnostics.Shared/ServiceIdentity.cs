namespace Basyc.Diagnostics.Shared;

/// <summary>
/// Identifies Service that is owner/producer of diagnostics data (logs, acitvities/spans etc.).
/// </summary>
public record struct ServiceIdentity(string ServiceName)
{
    public static ServiceIdentity ApplicationWideIdentity { get; set; } = new("Not specified identity");
}
