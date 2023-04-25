using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class SetupTypeFormattingStage
{
    public SetupTypeFormattingStage(IServiceCollection services)
    {
        Services = services;
        SetDefaultFormatting();
    }

    public IServiceCollection Services { get; init; }

    /// <summary>
    /// Removes all formatters.
    /// </summary>
    public SetupTypeFormattingStage ResetFormatting()
    {
        Services.RemoveAll<ITypedDomainNameFormatter>();
        Services.RemoveAll<ITypedRequestNameFormatter>();
        Services.RemoveAll<ITypedParameterNameFormatter>();
        Services.RemoveAll<ITypedResponseNameFormatter>();
        Services.RemoveAll<IResponseFormatter>();
        SetDefaultFormatting();
        return this;
    }

    public SetupTypeFormattingStage SetDomainNameFormatter<TDomainNameFormatter>() where TDomainNameFormatter : class, ITypedDomainNameFormatter
    {
        Services.RemoveAll<ITypedDomainNameFormatter>();
        Services.AddSingleton<ITypedDomainNameFormatter, TDomainNameFormatter>();
        return this;
    }

    public SetupTypeFormattingStage SetRequestNameFormatter<TRequestNameFormatter>() where TRequestNameFormatter : class, ITypedRequestNameFormatter
    {
        Services.RemoveAll<ITypedRequestNameFormatter>();
        Services.AddSingleton<ITypedRequestNameFormatter, TRequestNameFormatter>();
        return this;
    }

    public SetupTypeFormattingStage SetParamaterNameFormatter<TParameterTypeNameFormatter>() where TParameterTypeNameFormatter : class, ITypedParameterNameFormatter
    {
        Services.RemoveAll<ITypedParameterNameFormatter>();
        Services.AddSingleton<ITypedParameterNameFormatter, TParameterTypeNameFormatter>();
        return this;
    }

    public SetupTypeFormattingStage SetResponseNameFormatter<TResponseNameFormatter>() where TResponseNameFormatter : class, ITypedResponseNameFormatter
    {
        Services.RemoveAll<ITypedResponseNameFormatter>();
        Services.AddSingleton<ITypedResponseNameFormatter, TResponseNameFormatter>();
        return this;
    }

    public SetupTypeFormattingStage SetResponseFormatter<TResponseFormatter>() where TResponseFormatter : class, IResponseFormatter
    {
        Services.RemoveAll<IResponseFormatter>();
        Services.AddSingleton<IResponseFormatter, TResponseFormatter>();
        return this;
    }

    private void SetDefaultFormatting()
    {
        SetDomainNameFormatter<TypedDomainNameFormatter>();
        SetRequestNameFormatter<TypedRequestNameFormatter>();
        SetParamaterNameFormatter<TypedParameterTypeNameFormatter>();
        SetResponseNameFormatter<TypedResponseNameFormatter>();
        SetResponseFormatter<JsonResponseFormatter>();
    }
}
