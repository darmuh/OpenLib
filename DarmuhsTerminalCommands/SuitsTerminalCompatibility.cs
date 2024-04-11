using static suitsTerminal.AdvancedMenu;

namespace TerminalStuff
{
    internal class SuitsTerminalCompatibility
    {
        internal static void CheckForSuitsMenu()
        {
            if(!Plugin.instance.suitsTerminal)
                return;

            if (specialMenusActive)
            {
                ShortcutBindings.stopForAnyReason = true;
            }
            else
            {
                ShortcutBindings.stopForAnyReason = false;
            }
        }
    }
}
