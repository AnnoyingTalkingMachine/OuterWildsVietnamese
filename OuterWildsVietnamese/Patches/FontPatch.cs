using HarmonyLib;
using OWML.ModHelper;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OuterWildsVietnamese
{
    [HarmonyPatch]
    public static class FontPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenManager), nameof(TitleScreenManager.FadeInMenuOptions))]
        public static void FadeInMenuOptionsPostfix(TitleScreenManager __instance)
        {
            if (OuterWildsVietnamese.menuFont != null)
                for (int i = 0; i < __instance._mainMenuTextFields.Length; i++)
                {
                    __instance._mainMenuTextFields[i].font = OuterWildsVietnamese.menuFont;
                }
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Menu), nameof(Menu.InitializeMenu))]
        //public static void InitializeMenuPostfix(Menu __instance)
        //{
        //    MenuOption[] list = __instance.GetMenuOptions();
        //    if (OuterWildsVietnamese.menuFont != null)
        //        foreach (MenuOption option in list)
        //        {
        //            option.GetLabelField().font = OuterWildsVietnamese.menuFont;
        //            //Luminescence090923: Can't change the font of SecondaryTextField and TooltipDisplay

        //            //if (option.GetSecondaryTextField().font != null)
        //            //    option.GetSecondaryTextField().font = OuterWildsVietnamese.menuFont;
        //            //option._menuTooltipDisplay._textDisplay.font = OuterWildsVietnamese.menuFont;
        //        }
        //}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Menu), nameof(Menu.Activate))]
        public static void ActivatePostfix(Menu __instance)
        {
            MenuOption[] list = __instance.GetMenuOptions();
            if (OuterWildsVietnamese.menuFont != null)
                foreach (MenuOption option in list)
                {
                    option.GetLabelField().font = OuterWildsVietnamese.menuFont;
                    //Luminescence090923: Can't change the font of SecondaryTextField and TooltipDisplay

                    //if (option.GetSecondaryTextField().font != null)
                    //    option.GetSecondaryTextField().font = OuterWildsVietnamese.menuFont;
                    //option._menuTooltipDisplay._textDisplay.font = OuterWildsVietnamese.menuFont;
                }
        }

        //GameOver Font patch
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameOverController), nameof(GameOverController.SetupGameOverScreen))]
        public static bool SetupGameOverScreenPrefix(GameOverController __instance)
        {
            if (OuterWildsVietnamese.menuFont != null)
                __instance._deathText.font = OuterWildsVietnamese.menuFont;
            else
                OuterWildsVietnamese.Log("Main Menu Font (GameOver) null");
            return true;
        }

        //Conversation Font patch
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DialogueBoxVer2), "InitializeFont")]
        public static bool InitializeFontPrefix(DialogueBoxVer2 __instance)
        {
            if (OuterWildsVietnamese.conversationFont != null)
            {
                //__instance._fontInUse = OuterWildsVietnamese.bundle.LoadAsset<Font>("Assets/MerriweatherSans-SemiBold.ttf");
                __instance._fontInUse = OuterWildsVietnamese.conversationFont;
                __instance._dynamicFontInUse = OuterWildsVietnamese.conversationFont;
            }
            else
            {
                OuterWildsVietnamese.Log("Heathian Font null");
                return true;
            }
            int customFontSize = 32;
            float customFontSpacing = 1.4f;

            //__instance._dynamicFontInUse = __instance._defaultDialogueFontDynamic;
            __instance._fontSpacingInUse = __instance._defaultFontSpacing;
            __instance._mainTextField.font = __instance._fontInUse;
            __instance._mainTextField.lineSpacing = __instance._fontSpacingInUse * customFontSpacing;
            __instance._mainTextField.fontSize = customFontSize;
            __instance._nameTextField.font = __instance._fontInUse;
            __instance._nameTextField.lineSpacing = __instance._fontSpacingInUse * customFontSpacing;
            __instance._nameTextField.fontSize = customFontSize;

            DialogueOptionUI requiredComponent =
                __instance._optionBox.GetRequiredComponent<DialogueOptionUI>();
            requiredComponent.textElement.font = __instance._fontInUse;
            requiredComponent.textElement.lineSpacing = __instance._fontSpacingInUse * customFontSpacing;
            requiredComponent.textElement.fontSize = customFontSize;
            return false;
        }

        //Prompt Font patch
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PromptManager), nameof(PromptManager.Awake))]
        public static void PromptManagerPostfix(PromptManager __instance)
        {
            if (OuterWildsVietnamese.promptFont != null)
                __instance._currentFont = OuterWildsVietnamese.promptFont;
            else
                OuterWildsVietnamese.Log("Prompt Font null");
        }
    }
}
