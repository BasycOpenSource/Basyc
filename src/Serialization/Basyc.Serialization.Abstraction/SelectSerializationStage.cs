namespace Microsoft.Extensions.DependencyInjection;

public class SelectSerializationStage
{
    public readonly IServiceCollection services;

    public SelectSerializationStage(IServiceCollection services)
    {
        this.services = services;
    }
}
