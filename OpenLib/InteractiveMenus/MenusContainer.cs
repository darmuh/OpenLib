using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenLib.InteractiveMenus
{
    public class MenusContainer
    {
        public static List<BetterMenuBase> AllMenus = [];

        public static bool TryGetMenu(string menuName, out BetterMenuBase item)
        {

            item = AllMenus.FirstOrDefault(x => x.Name == menuName);

            if (item == null)
                return false;
            else
                return true;
        }

        public static bool AnyMenuActive()
        {
            bool external = false;

            if (Plugin.instance.ITAPI)
                external = Compat.InteractiveTermAPI.ApplicationInUse();

            if (external)
                return true;

            if (AllMenus.Count == 0)
                return false;

            if (AllMenus.Any(x => x.InMenu))
                return true;

            return false;
        }
    }
}
