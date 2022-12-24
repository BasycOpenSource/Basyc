namespace Basyc.MessageBus.Manager.Infrastructure.Formatters;

public interface IResponseFormatter
{
	string Format(object response);
}
