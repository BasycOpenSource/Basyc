using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class SetupTypeFormattingStage
{
	public readonly IServiceCollection services;

	public SetupTypeFormattingStage(IServiceCollection services)
	{
		this.services = services;
		SetDefaultFormatting();
	}

	private void SetDefaultFormatting()
	{
		SetDomainNameFormatter<TypedDomainNameFormatter>();
		SetRequestNameFormatter<TypedRequestNameFormatter>();
		SetParamaterNameFormatter<TypedParameterTypeNameFormatter>();
		SetResponseNameFormatter<TypedResponseNameFormatter>();
		SetResponseFormatter<JsonResponseFormatter>();
	}

	///Removes all formatters
	public SetupTypeFormattingStage ResetFormatting()
	{
		services.RemoveAll<ITypedDomainNameFormatter>();
		services.RemoveAll<ITypedRequestNameFormatter>();
		services.RemoveAll<ITypedParameterNameFormatter>();
		services.RemoveAll<ITypedResponseNameFormatter>();
		services.RemoveAll<IResponseFormatter>();
		SetDefaultFormatting();
		return this;
	}

	public SetupTypeFormattingStage SetDomainNameFormatter<TDomainNameFormatter>() where TDomainNameFormatter : class, ITypedDomainNameFormatter
	{
		services.RemoveAll<ITypedDomainNameFormatter>();
		services.AddSingleton<ITypedDomainNameFormatter, TDomainNameFormatter>();
		return this;
	}

	public SetupTypeFormattingStage SetRequestNameFormatter<TRequestNameFormatter>() where TRequestNameFormatter : class, ITypedRequestNameFormatter
	{
		services.RemoveAll<ITypedRequestNameFormatter>();
		services.AddSingleton<ITypedRequestNameFormatter, TRequestNameFormatter>();
		return this;
	}

	public SetupTypeFormattingStage SetParamaterNameFormatter<TParameterTypeNameFormatter>() where TParameterTypeNameFormatter : class, ITypedParameterNameFormatter
	{
		services.RemoveAll<ITypedParameterNameFormatter>();
		services.AddSingleton<ITypedParameterNameFormatter, TParameterTypeNameFormatter>();
		return this;
	}

	public SetupTypeFormattingStage SetResponseNameFormatter<TResponseNameFormatter>() where TResponseNameFormatter : class, ITypedResponseNameFormatter
	{
		services.RemoveAll<ITypedResponseNameFormatter>();
		services.AddSingleton<ITypedResponseNameFormatter, TResponseNameFormatter>();
		return this;
	}

	public SetupTypeFormattingStage SetResponseFormatter<TResponseFormatter>() where TResponseFormatter : class, IResponseFormatter
	{
		services.RemoveAll<IResponseFormatter>();
		services.AddSingleton<IResponseFormatter, TResponseFormatter>();
		return this;
	}
}