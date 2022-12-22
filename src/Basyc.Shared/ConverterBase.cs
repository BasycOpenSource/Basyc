namespace Basyc.Shared;

public abstract class ConverterBase<TSource, TTarget> : IConverter<TSource, TTarget>
{
    public abstract TSource ToSource(TTarget target);

    public List<TSource> ToSources(IEnumerable<TTarget> targets)
    {
        List<TSource> sources = new();
        foreach (TTarget? target in targets)
        {
            sources.Add(ToSource(target));
        }

        return sources;
    }

    public List<TTarget> ToTargets(IEnumerable<TSource> sources)
    {
        List<TTarget> targets = new();
        foreach (TSource? source in sources)
        {
            targets.Add(ToTarget(source));
        }

        return targets;
    }

    public string GetString()
    {
        string text;
        text = "";
        Console.WriteLine(text);
        return "";
    }

    public abstract TTarget ToTarget(TSource source);

}
