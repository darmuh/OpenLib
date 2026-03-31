

using Dawn.Utils;

namespace OpenLib.Compat;

internal class DawnlibCompat
{
    public static void SetCommandPriority(TerminalKeyword keyword)
    {
        if (!Plugin.instance.DawnLibPresent)
            return;

        keyword.SetKeywordPriority(Dawn.ITerminalKeyword.DawnKeywordType.DawnCommand);
    }
}
