using HarmonyLib;
using OpenLib.Events;
using OpenLib.InteractiveMenus;
using UnityEngine.InputSystem;

namespace OpenLib
{

    [HarmonyPatch(typeof(Terminal), "Awake")]
    public class AwakeTermPatch : Terminal
    {
        static void Postfix(Terminal __instance)
        {
            EventManager.TerminalAwake.Invoke(__instance);
            //terminal awake event
        }
    }

    //Terminal disabled, disabling ESC key listener OnDisable
    [HarmonyPatch(typeof(Terminal), "OnDisable")]
    public class DisableTermPatch : Terminal
    {
        static void Postfix()
        {
            //remove keywords
            //terminal disabled event   
            EventManager.TerminalDisable.Invoke();
        }
    }

    [HarmonyPatch(typeof(Terminal), "QuitTerminal")]
    public class QuitTerminalPatch : Terminal
    {
        static void Postfix()
        {
            //terminal quit  
            EventManager.TerminalQuit.Invoke();
        }
    }


    [HarmonyPatch(typeof(Terminal), "LoadNewNode")]
    public class LoadNewNodePatch : Terminal
    {
        static void Postfix(TerminalNode node)
        {
            EventManager.TerminalLoadNewNode.Invoke(node);
        }
    }

    [HarmonyPatch(typeof(Terminal), "Start")]
    public class TerminalStartPatch : Terminal
    {
        static void Postfix()
        {
            //start event
            EventManager.TerminalStart.Invoke();
        }
    }

    [HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
    public class Terminal_Begin_Patch
    {
        static void Postfix()
        {
            //put event here
            EventManager.TerminalBeginUsing.Invoke();
        }
    }

    [HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
    public class Terminal_ParsePlayerSentence_Patch
    {
        static void Postfix(ref TerminalNode __result)
        {
            //event
            if (__result == null)
                return;

            TerminalNode node = EventManager.TerminalParseSent.NodeInvoke(ref __result);
            __result = node;
            return;
        }
    }


    [HarmonyPatch(typeof(Terminal), "Update")]
    public class TerminalUpdatePatch
    {
        public static bool inUse = false;
        public static bool usePatch = false;
        public static int menuIndex = 0;

        static void Postfix(Terminal __instance)
        {
            if (!usePatch) //any mod that wishes to use this patch needs to enable this
                return;

            //events
            if (__instance.placeableObject.inUse != inUse)
            {
                inUse = __instance.placeableObject.inUse;
                EventManager.SetTerminalInUse.Invoke();
            }
            /*
            if(Keyboard.current.anyKey.wasPressedThisFrame && __instance.terminalInUse)
            {
                EventManager.TerminalKeyPressed.Invoke();
            }*/

        }

        [HarmonyPrefix]
        static bool Prefix(Terminal __instance)
        {
            if (!usePatch || !__instance.terminalInUse) //any mod that wishes to use this patch needs to enable this
                return true;

            if (MenusContainer.AnyOpenLibMenuActive())
            {
                __instance.scrollBarCanvasGroup.alpha = 0f; //hide scrollbar
                __instance.topRightText.text = $"${__instance.groupCredits}";
                //events
                if (__instance.placeableObject.inUse != inUse)
                {
                    inUse = __instance.placeableObject.inUse;
                    EventManager.SetTerminalInUse.Invoke();
                }

                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    EventManager.TerminalMenuKeyPressed.Invoke();
                }

                return false;
            }

            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                //for non menu listeners like shortcuts
                EventManager.TerminalKeyPressed.Invoke();
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Terminal), "LoadNewNodeIfAffordable")]
    public class AffordableNodePatch
    {
        static void Postfix(TerminalNode node)
        {
            //events
            EventManager.TerminalLoadIfAffordable.Invoke(node);
        }
    }

}