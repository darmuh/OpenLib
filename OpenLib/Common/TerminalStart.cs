using OpenLib.Events;
using System.Collections;
using UnityEngine;

namespace OpenLib.Common
{
    public class TerminalStart
    {
        internal static bool delayStartEnum = false;

        internal static void TerminalStartGroupDelay()
        {
            Loggers.LogDebug("Starting TerminalDelayStartEnumerator");
            Plugin.instance.Terminal.StartCoroutine(TerminalDelayStartEnumerator());
        }

        internal static IEnumerator TerminalDelayStartEnumerator()
        {
            if (delayStartEnum)
                yield break;

            delayStartEnum = true;

            yield return new WaitForSeconds(1);
            Loggers.LogInfo("1 Second delay methods starting.");
            EventManager.TerminalDelayStart.Invoke();
            AddStoreItems(); //adding after delay for storerotation mod
            delayStartEnum = false;
        }

        internal static void AddStoreItems()
        {
            if (Plugin.instance.TerminalFormatter)
                return;

            AddShopItemsToFurnitureList();
        }

        private static void AddShopItemsToFurnitureList()
        {
            foreach (TerminalNode shopNode in Plugin.ShopNodes)
            {

                if (!Plugin.instance.Terminal.ShipDecorSelection.Contains(shopNode))
                {
                    Plugin.instance.Terminal.ShipDecorSelection.Add(shopNode);
                    Loggers.LogDebug($"adding {shopNode.creatureName} to shipdecorselection");
                }
                else
                {
                    Loggers.LogDebug($"{shopNode.creatureName} already in shipdecorselection");
                }
            }

            Loggers.LogDebug("nodes have been added");
        }
    }
}
