namespace Microsoft.Extensions.DependencyInjection;

public class SelectSerializationStage
{
    public SelectSerializationStage(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; init; }
}
