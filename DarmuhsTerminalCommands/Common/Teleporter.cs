using OpenLib.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLib.Common
{
    public class Teleporter
    {
        public static ShipTeleporter NormalTP;
        public static ShipTeleporter InverseTP;

        public static void CheckTeleporterTypeAndAssign(ShipTeleporter instance)
        {
            if (instance.isInverseTeleporter)
            {
                ITPexists(instance);
            }
            else
            {
                TPexists(instance);
            }
        }

        public static void TPexists(ShipTeleporter instance)
        {
            NormalTP = instance;
            Plugin.MoreLogs("NormalTP instance detected and set.");
            EventManager.NormalTPFound.Invoke();
        }

        public static void ITPexists(ShipTeleporter instance)
        {
            InverseTP = instance;
            Plugin.MoreLogs("InverseTP instance detected and set.");
            EventManager.InverseTPFound.Invoke();
        }
    }
}
