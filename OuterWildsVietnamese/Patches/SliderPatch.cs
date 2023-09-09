using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace OuterWildsVietnamese
{
    [HarmonyPatch]
    public static class SliderPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.Init))]
        private static void PlayerData_Init(SettingsSave settingsData)
        {
            LanguageSaveFile save = OuterWildsVietnamese.Load();
            if (save != null && !string.IsNullOrWhiteSpace(save.language))
            {
                settingsData.language = OuterWildsVietnamese.vietnamese;
            }
        }

        /// Avoid save breaking
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.SaveSettings))]
        private static void PlayerData_SaveSettings_Prefix()
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(PlayerData._settingsSave.language))
            {
                OuterWildsVietnamese.Save(new LanguageSaveFile(string.Empty));
                return;
            }
            PlayerData._settingsSave.language = OuterWildsVietnamese.languageToReplace;
            OuterWildsVietnamese.Save(new LanguageSaveFile("Vietnamese"));
        }

        /// Set back to saved custom language
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.SaveSettings))]
        private static void PlayerData_SaveSettings_Postfix()
        {
            LanguageSaveFile save = OuterWildsVietnamese.Load();
            if (save != null && !string.IsNullOrEmpty(save.language))
            {
                PlayerData._settingsSave.language = OuterWildsVietnamese.vietnamese;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.GetSavedLanguage))]
        private static bool PlayerData_GetSavedLanguage(ref TextTranslation.Language __result)
        {
            LanguageSaveFile save = OuterWildsVietnamese.Load();
            if (save != null && !string.IsNullOrWhiteSpace(save.language))
            {
                __result = OuterWildsVietnamese.vietnamese;
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsSave), nameof(SettingsSave.GetLanguageStrings))]
        private static void SettingsSave_GetLanguageStrings(ref string[] __result)
        {
            Array.Resize(ref __result, (int)TextTranslation.Language.TOTAL + 2);
            __result[(int)TextTranslation.Language.TOTAL] = "Total";
            __result[(int)OuterWildsVietnamese.vietnamese] = "Tiếng Việt";
        }

        /// For skipping TextTranslation.Language.TOTAL
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OptionsSelectorElement), nameof(OptionsSelectorElement.OptionsMove))]
        private static void OptionsSelectorElement_OptionsMove(OptionsSelectorElement __instance, Vector2 moveVector)
        {
            if (__instance._settingId == SettingsID.LANGUAGE)
            {
                bool increasing = __instance._directionality == OptionsSelectorElement.Direction.HORIZONTAL ? moveVector.x > 0 : (__instance._directionality == OptionsSelectorElement.Direction.VERTICAL ? moveVector.y < 0 : false);
                bool decreasing = __instance._directionality == OptionsSelectorElement.Direction.HORIZONTAL ? moveVector.x < 0 : (__instance._directionality == OptionsSelectorElement.Direction.VERTICAL ? moveVector.y > 0 : false);
                if ((increasing && __instance._value == (int)TextTranslation.Language.TOTAL - 1) || (decreasing && __instance._value == (int)TextTranslation.Language.TOTAL + 1))
                {
                    __instance._value = (int)TextTranslation.Language.TOTAL;
                }
            }

        }

        public class LanguageSaveFile
        {
            public readonly string language;
            public LanguageSaveFile(string language) => this.language = language;

            public static readonly LanguageSaveFile DEFAULT = new LanguageSaveFile(string.Empty);
        }
    }
}