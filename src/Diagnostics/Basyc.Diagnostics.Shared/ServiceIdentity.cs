namespace Basyc.Diagnostics.Shared;

/// <summary>
/// Identifies Service that is owner/producer of diagnostics data (logs, acitvities/spans etc.)
/// </summary>
/// <param name="ServiceName"></param>
public record struct ServiceIdentity(string ServiceName)
{
	public static ServiceIdentity ApplicationWideIdentity = new ServiceIdentity("Not specified identity");
}
