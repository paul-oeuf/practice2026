namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        var cleaned = new string(
            input
                .ToLower()
                .Where(c =>
                    !char.IsWhiteSpace(c) &&
                    !char.IsPunctuation(c))
                .ToArray());

        var reversed = new string(cleaned.Reverse().ToArray());

        return cleaned == reversed;
    }
}