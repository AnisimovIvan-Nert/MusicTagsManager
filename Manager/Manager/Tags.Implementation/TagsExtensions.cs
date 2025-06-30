namespace Manager.Tags.Implementation;

internal static class TagsExtensions
{
    public static string[]? DeterminateNewValue(this string[]? oldValue, string? firstElement)
    {
        if (firstElement == null)
            return null;

        if (oldValue == null || oldValue.Length == 0)
            return [firstElement];

        oldValue[0] = firstElement;
        return oldValue;
    }
}