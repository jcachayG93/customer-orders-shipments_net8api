namespace WebApi.Tests.TestCommon;

public static class TestCollectionExtensions
{
    public static ICollection<T> ToCollection<T>(this T item, params T[] additionalItems)
    {
        List<T> result = new List<T>() { item };

        foreach (T x in additionalItems)
        {
            result.Add(x);
        }

        return result;
    }
}