namespace Basyc.Shared;

public interface IConverter<TSource, TTarget>
{
    TSource ToSource(TTarget target);

    TTarget ToTarget(TSource source);

    List<TSource> ToSources(IEnumerable<TTarget> targets);

    List<TTarget> ToTargets(IEnumerable<TSource> sources);
}
