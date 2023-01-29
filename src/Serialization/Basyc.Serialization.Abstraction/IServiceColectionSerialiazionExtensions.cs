namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceColectionSerialiazionExtensions
{
	public static SelectSerializationStage AddBasycSerialization(this IServiceCollection service)
	{
		return new SelectSerializationStage(service);
	}
}
