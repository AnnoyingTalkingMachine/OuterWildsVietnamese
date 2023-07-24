using HarmonyLib;
using UnityEngine;

namespace OuterWildsVietnamese
{
    [HarmonyPatch]
    public static class FontPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetFont))]
        public static bool GetFont(ref Font __result)
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(TextTranslation.Get().GetLanguage())) return true;

            __result = OuterWildsVietnamese.heathianConversationFont;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStyleManager), nameof(UIStyleManager.GetMenuFont))]
        public static bool GetMenuFont(ref Font __result)
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(PlayerData.GetSavedLanguage())) return true;

            __result = OuterWildsVietnamese.mainMenuFont;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameOverController), nameof(GameOverController.SetupGameOverScreen))]
        public static bool SetupGameOverScreen(GameOverController __instance)
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(TextTranslation.Get().GetLanguage())) return true;

            __instance._deathText.font = OuterWildsVietnamese.creditsFont;
            return false;
        }
    }
}
