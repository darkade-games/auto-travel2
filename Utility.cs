using System.Linq;
using System.Text.RegularExpressions;

namespace AutoTravel2;

static class Utility
{
    public static string[] SpliceText(string text, int lineLength)
    {
        return Regex.Matches(text, ".{1," + lineLength + "}").Cast<Match>().Select(m => m.Value).ToArray();
    }
}
