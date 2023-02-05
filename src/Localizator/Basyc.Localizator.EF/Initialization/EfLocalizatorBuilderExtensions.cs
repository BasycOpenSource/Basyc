using Basyc.Localizator.Abstraction.Initialization;

namespace Basyc.Localizator.Infrastructure.EF.Initialization;

public static class EfLocalizatorBuilderExtensions
{
	public static LocalizatorBuilder AddEfStorage(this LocalizatorBuilder builder)
	{
		builder.AddStorage<EfLocalizatorStorage>();
		return builder;
	}
}
