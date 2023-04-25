namespace Basyc.MessageBus.Manager.Application;

public interface ITypedResponseNameFormatter
{
    public string GetFormattedName(Type responseType);
}
