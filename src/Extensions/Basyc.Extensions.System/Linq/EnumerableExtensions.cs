namespace System.Linq;

public static class EnumerableExtensions
{
    public static int WhereIndex<T>(this IEnumerable<T> source, Func<T, bool> someCondition) => source.TakeWhile(t => !someCondition(t)).Count();
}
