namespace Microsoft.Extensions.DependencyInjection;

public class SelectSerializationStage
{
    public SelectSerializationStage(IServiceCollection services)
    {
        this.Services = services;
    }

    public IServiceCollection Services { get; init; }
}
