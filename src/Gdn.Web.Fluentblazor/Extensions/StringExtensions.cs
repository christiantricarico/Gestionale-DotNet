using System.Text;

namespace Gdn.Web.Fluentblazor.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Remove all but the last occurrence of a character from a string.
    /// </summary>
    /// <param name="input">String to evalueate</param>
    /// <param name="target">Character to remove</param>
    /// <returns>The resulting string</returns>
    public static string RemoveAllButLast(this string input, char target)
    {
        int lastIndex = input.LastIndexOf(target);

        var result = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] != target || i == lastIndex)
                result.Append(input[i]);
        }

        return result.ToString();
    }
}
