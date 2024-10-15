

namespace OpenLib.Compat
{
    public class ModelAPI
    {
        public static int GetFirstPersonMask(int originalMask)
        {
            if (!Plugin.instance.ModelReplacement)
                return 0;

            // Masks for each rendering mode
            int MaskModel = (1 << ModelReplacement.ViewStateManager.modelLayer) + (1 << ModelReplacement.ViewStateManager.NoPostModelLayer);
            int MaskArms = (1 << ModelReplacement.ViewStateManager.armsLayer);
            int MaskVisible = (1 << ModelReplacement.ViewStateManager.visibleLayer) + (1 << ModelReplacement.ViewStateManager.NoPostVisibleLayer);

            originalMask = originalMask | MaskArms; // Turn on arms
            originalMask = originalMask & ~MaskModel; // Turn off model
            originalMask = originalMask | MaskVisible; // Turn on visible

            return originalMask;
        }

        public static int GetThirdPersonMask(int originalMask)
        {
            if (!Plugin.instance.ModelReplacement)
                return 0;

            // Masks for each rendering mode
            int MaskModel = (1 << ModelReplacement.ViewStateManager.modelLayer) + (1 << ModelReplacement.ViewStateManager.NoPostModelLayer);
            int MaskArms = (1 << ModelReplacement.ViewStateManager.armsLayer);
            int MaskVisible = (1 << ModelReplacement.ViewStateManager.visibleLayer) + (1 << ModelReplacement.ViewStateManager.NoPostVisibleLayer);

            originalMask = originalMask & ~MaskArms; // Turn off arms
            originalMask = originalMask | MaskModel; // Turn on model
            originalMask = originalMask | MaskVisible; // Turn on visible

            return originalMask;
        }
    }
}
