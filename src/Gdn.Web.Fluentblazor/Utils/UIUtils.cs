using Gdn.Web.Fluentblazor.Extensions;

namespace Gdn.Web.Fluentblazor.Utils;

public static class UIUtils
{
    public static decimal ConvertTextToDecimal(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        text = text.Replace(".", ",").RemoveAllButLast(',');

        try
        {
            decimal value = Convert.ToDecimal(text);
            return value;
        }
        catch (FormatException)
        {
            return 0;
        }
        catch (OverflowException)
        {
            return 0;
        }
    }
}
