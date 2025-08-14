using OpenLib.CoreMethods;
using System.Collections.Generic;
using System.Linq;

namespace OpenLib.InteractiveMenus
{
    public class MenusContainer
    {
        public static List<BetterMenuBase> AllMenus = [];
        private static int _activeIndex = 0;
        public static int StaticIndex
        {
            get
            {


                return _activeIndex;
            }
            set
            {
                _activeIndex = value;
            }
        }

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
            if (AllInteractiveMenus.AnyInteractiveMenuActive()) //required for interactive menus compatibility
                return true;

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

        public static bool AnyBetterMenuActive()
        {
            if (AllMenus.Count == 0)
                return false;

            if (AllMenus.Any(x => x.InMenu))
                return true;

            return false;
        }

        public static bool AnyOpenLibMenuActive()
        {
            if (AllInteractiveMenus.AnyInteractiveMenuActive()) //required for interactive menus compatibility
                return true;

            if (AllMenus.Count == 0)
                return false;

            if (AllMenus.Any(x => x.InMenu))
                return true;

            return false;
        }
    }
}
