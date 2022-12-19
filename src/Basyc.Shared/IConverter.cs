using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Shared
{
    public interface IConverter<TSource, TTarget>
    {
        TSource ToSource(TTarget target);
        TTarget ToTarget(TSource source);

        List<TSource> ToSources(IEnumerable<TTarget> targets);
        List<TTarget> ToTargets(IEnumerable<TSource> sources);
    }
}