using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class SetupMessagesStageInterfaceExtensions
{
    // public static SetupGroupStage FromAssemblyScan(this SetupMessagesStage parent, params Assembly[] assembliesToScan)
    // {
    //  return new SetupGroupStage(parent.services, assembliesToScan);
    // }

    public static TypeFilterStage FromAssemblyScan(this SetupMessagesStage parent, params Assembly[] assembliesToScan) => new(parent.Services, assembliesToScan);

    public class TypeFilterStage : BuilderStageBase
    {
        private readonly Assembly[] assemblies;

        public TypeFilterStage(IServiceCollection services, Assembly[] assemblies) : base(services)
        {
            this.assemblies = assemblies;
        }

        public TypesToRegisterStage Where(Func<Type, bool> filter)
        {
            var filteredTypes = assemblies.SelectMany(x => x.DefinedTypes).Where(filter).ToArray();
            return new TypesToRegisterStage(Services, filteredTypes);
        }

        public TypesToRegisterStage WhereImplements<TImplementedType>()
        {
            var filteredTypes = assemblies.SelectMany(x => x.DefinedTypes).Where(x => x.GetInterface(typeof(TImplementedType).Name) is not null).ToArray();
            return new TypesToRegisterStage(Services, filteredTypes);
        }

        public TypesToRegisterStage WhereImplements(Type implementedType)
        {
            var filteredTypes = assemblies.SelectMany(x => x.DefinedTypes).Where(x => x.GetInterface(implementedType.Name) is not null).ToArray();
            return new TypesToRegisterStage(Services, filteredTypes);
        }
    }

    public class TypesToRegisterStage : BuilderStageBase
    {
        private readonly Type[] types;

        public TypesToRegisterStage(IServiceCollection services, Type[] types) : base(services)
        {
            this.types = types;
        }

        public void Register(Action<Type, FluentAddGroupStage> register)
        {
            foreach (var type in types)
                register.Invoke(type, new FluentAddGroupStage(Services));
        }
    }
}
