using OpenLib.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenLib.Common
{
    public class TerminalStart
    {
        public static bool delayStartEnum = false;

        internal static void TerminalStartGroupDelay()
        {
            Plugin.Spam("Starting TerminalDelayStartEnumerator");
            Plugin.instance.Terminal.StartCoroutine(TerminalDelayStartEnumerator());
        }

        internal static IEnumerator TerminalDelayStartEnumerator()
        {
            if (delayStartEnum)
                yield break;

            delayStartEnum = true;

            yield return new WaitForSeconds(1);
            Plugin.MoreLogs("1 Second delay methods starting.");
            EventManager.TerminalDelayStart.Invoke();
            AddStoreItems(); //adding after delay for storerotation mod
            delayStartEnum = false;
        }

        public static void AddStoreItems()
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
                    Plugin.Spam($"adding {shopNode.creatureName} to shipdecorselection");
                }
                else
                {
                    Plugin.Spam($"{shopNode.creatureName} already in shipdecorselection");
                }
            }

            Plugin.Spam("nodes have been added");
        }
    }
}
