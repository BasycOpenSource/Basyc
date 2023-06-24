namespace System;

public static class IntExtensions
{
    public static void Times(this int count, Action<int> action)
    {
        for (int i = 0; i < count; i++)
        {
            action(i);
        }
    }
}
