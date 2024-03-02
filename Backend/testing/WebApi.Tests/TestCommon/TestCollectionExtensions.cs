namespace WebApi.Tests.TestCommon;

public static class TestCollectionExtensions
{
    public static ICollection<T> ToCollectionWith<T>(this T item, params T[] additionalItems)
    {
        var result = new List<T>() { item };

        foreach (var x in additionalItems)
        {
            result.Add(x);
        }

        return result;
    }
}