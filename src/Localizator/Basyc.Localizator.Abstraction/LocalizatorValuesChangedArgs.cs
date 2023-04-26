namespace Basyc.Localizator.Abstraction;

public class LocalizatorValuesChangedArgs : EventArgs
{
    public LocalizatorValuesChangedArgs(IDictionary<string, string> newValues)
    {
        NewValues = newValues;
    }

    public IDictionary<string, string> NewValues { get; }
}
