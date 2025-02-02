using InteractiveTerminalAPI.UI;

namespace OpenLib.Compat
{
    public class InteractiveTermAPI
    {
        public static bool ApplicationInUse()
        {
            if (!Plugin.instance.ITAPI)
                return false;

            return InteractiveTerminalManager.InteractiveTerminalBeingUsed();
        }
    }
}
