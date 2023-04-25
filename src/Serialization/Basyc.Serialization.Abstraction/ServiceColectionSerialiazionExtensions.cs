namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceColectionSerialiazionExtensions
{
    public static SelectSerializationStage AddBasycSerialization(this IServiceCollection service) => new(service);
}
