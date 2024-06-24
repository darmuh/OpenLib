using static TerminalStuff.AlwaysOnStuff;
using static TerminalStuff.AllMyTerminalPatches;
using static TerminalStuff.TerminalClockStuff;
using static TerminalStuff.ShipControls;
using static TerminalStuff.TerminalEvents;
using static TerminalStuff.DynamicCommands;
using static TerminalStuff.ShortcutBindings;
using static TerminalStuff.NetHandler;
using static TerminalStuff.AdminCommands;
using static TerminalStuff.WalkieTerm;
using static TerminalStuff.StartofHandling;

namespace TerminalStuff
{
    internal class BoolStuff
    {
        internal static bool ListenForShortCuts()
        {

            if (Plugin.instance.suitsTerminal && SuitsTerminalCompatibility.CheckForSuitsMenu())
                return false;

            if (!Plugin.instance.Terminal.terminalInUse)
                return false;

            return true;
        }

        internal static void ResetEnumBools()
        {
            TerminalStartPatch.delayStartEnum = false;
            dynamicStatus = false;
            QuitPatch.videoQuitEnum = false;
            quitTerminalEnum = false;
            leverEnum = false;
            fovEnum = false;
            shortcutListenEnum = false;
            terminalClockEnum = false;
            rainbowFlashEnum = false;
            kickEnum = false;
            walkieEnum = false;
            textUpdater = false;
        }


    }
}
