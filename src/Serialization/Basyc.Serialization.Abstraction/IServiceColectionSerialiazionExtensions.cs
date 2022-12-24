namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceColectionSerialiazionExtensions
{
	public static SelectSerializationStage AddBasycSerialization(this IServiceCollection service)
	{
		return new SelectSerializationStage(service);
	}
}
