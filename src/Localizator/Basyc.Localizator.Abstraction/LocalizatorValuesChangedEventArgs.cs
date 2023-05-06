namespace Basyc.Localizator.Abstraction;

public class LocalizatorValuesChangedEventArgs : EventArgs
{
    public LocalizatorValuesChangedEventArgs(IDictionary<string, string> newValues)
    {
        NewValues = newValues;
    }

    public IDictionary<string, string> NewValues { get; }
}
