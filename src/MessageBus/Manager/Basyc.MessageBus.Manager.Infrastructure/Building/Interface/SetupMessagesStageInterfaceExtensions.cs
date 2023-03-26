using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class SetupMessagesStageInterfaceExtensions
{
	public static SetupGroupStage FromAssemblyScan(this SetupMessagesStage parent, params Assembly[] assembliesToScan)
	{
		return new SetupGroupStage(parent.services, assembliesToScan);
	}

	public static TypeFilterStage FromAssembly2(this SetupMessagesStage parent, params Assembly[] assembliesToScan)
	{
		return new TypeFilterStage(parent.services, assembliesToScan);
	}


	public class TypeFilterStage : BuilderStageBase
	{
		private readonly Assembly[] assemblies;

		public TypeFilterStage(IServiceCollection services, Assembly[] assemblies) : base(services)
		{
			this.assemblies = assemblies;
		}

		public TypesToRegisterStage Filter(Func<Type, bool> filter)
		{
			var filteredTypes = assemblies.SelectMany(x => x.DefinedTypes).Where(filter).ToArray();
			return new TypesToRegisterStage(services, filteredTypes);
		}

		public TypesToRegisterStage FilterOnInterface<TInterface>()
		{
			var filteredTypes = assemblies.SelectMany(x => x.DefinedTypes).Where(x => x.GetInterface(typeof(TInterface).Name) is not null).ToArray();
			return new TypesToRegisterStage(services, filteredTypes);
		}
	}

	public class TypesToRegisterStage : BuilderStageBase
	{
		private readonly Type[] types;

		public TypesToRegisterStage(IServiceCollection services, Type[] types) : base(services)
		{
			this.types = types;
		}

		public void Register(Action<Type, RegisterMessagesFromFluentApiStage> register)
		{
			foreach (var type in types)
			{
				register.Invoke(type, new RegisterMessagesFromFluentApiStage(services));
			}
		}
	}
}
