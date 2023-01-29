namespace Basyc.Localizator.Abstraction;

public record GetLocalizatorResult(bool LocalizatorFound, ILocalizator Localizator);
