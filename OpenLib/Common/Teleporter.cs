using OpenLib.Events;

namespace OpenLib.Common
{
    public class Teleporter
    {
        public static ShipTeleporter NormalTP = null!;
        public static ShipTeleporter InverseTP = null!;

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
            Loggers.LogInfo("NormalTP instance detected and set.");
            EventManager.NormalTPFound.Invoke();
        }

        public static void ITPexists(ShipTeleporter instance)
        {
            InverseTP = instance;
            Loggers.LogInfo("InverseTP instance detected and set.");
            EventManager.InverseTPFound.Invoke();
        }
    }
}
