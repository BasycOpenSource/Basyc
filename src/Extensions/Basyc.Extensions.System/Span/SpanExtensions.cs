namespace System;
public static class SpanExtensions
{
	public static IEnumerable<T[]> Split<T>(this ReadOnlySpan<T> span, T separator)
	{
		var items = new List<T[]>();
		int index = 0;
		while (true)
		{
			index = CustomIndexOf(span, in separator);
			if (index == -1)
			{
				items.Add(span.ToArray());
				break;
			}

			var childSpan = span.Slice(0, index);
			span = span.Slice(index + 1);
			items.Add(childSpan.ToArray());
		}

		return items;
	}

	private static int CustomIndexOf<T>(ReadOnlySpan<T> span, in T itemToFind)
	{
		for (int itemIndex = 0; itemIndex < span.Length; itemIndex++)
		{
			var item = span[itemIndex];
			if (object.Equals(item, itemToFind))
				return itemIndex;
		}

		return -1;
	}
}
