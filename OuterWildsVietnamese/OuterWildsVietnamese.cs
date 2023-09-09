using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Text;
using System.IO;
using System.Reflection;
using UnityEngine.UI;
using Steamworks;
using Epic.OnlineServices;
using HarmonyLib;
using static OuterWildsVietnamese.SliderPatch;

namespace OuterWildsVietnamese
{
    public class OuterWildsVietnamese : ModBehaviour
    {
        static OuterWildsVietnamese self;

        //Providing method to check if the current language is vanilla
        public static readonly HashSet<TextTranslation.Language> vanillaLanguages = new HashSet<TextTranslation.Language>(Enum.GetValues(typeof(TextTranslation.Language)).Cast<TextTranslation.Language>());
        internal static bool IsVanillaLanguage() => IsVanillaLanguage(TextTranslation.Get().GetLanguage());
        internal static bool IsVanillaLanguage(TextTranslation.Language language) => vanillaLanguages.Contains(language);

        public static TextTranslation.Language vietnamese;
        public static AssetBundle bundle;
        public static Font conversationFont;
        public static Font menuFont;
        public static Font creditsFont;
        public static Font promptFont;
        public static string translationPath;
        public static TextTranslation.Language languageToReplace;


        private void Awake()
        {
            self = this;
        }

        private void Start()
        {
            vietnamese = vanillaLanguages.Max() + 1;
            translationPath = self.ModHelper.Manifest.ModFolderPath + "assets/VietnameseTranslation.xml";
            languageToReplace = TextTranslation.Language.ENGLISH;

            //Loading font assetbundle and defining different font usages
            try
            {
                bundle = ModHelper.Assets.LoadBundle("assets/font");
                conversationFont = bundle.LoadAsset<Font>("Assets/SignikaNegative-SemiBold.ttf");
                menuFont = bundle.LoadAsset<Font>("Assets/Rowdies-Light.ttf");
                creditsFont = bundle.LoadAsset<Font>("Assets/JosefinSans-Regular.ttf");
                promptFont = bundle.LoadAsset<Font>("Assets/Dosis-Regular.ttf");
                bundle.Unload(false);
            }
            catch (Exception e)
            {
                Log("Couldn't load font bundle.");
                Log(e.ToString());
            }

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Log("Đã thêm bản dịch Tiếng Việt. Chúc bạn may mắn trên hành trình của mình!");
            Log("-Luminescence010823-");
        }
    

        public static void Log(string msg)
        {
            self.ModHelper.Console.WriteLine($"{msg}");
        }

        public static LanguageSaveFile Load() => self.ModHelper.Storage.Load<LanguageSaveFile>("languageSave.json") ?? LanguageSaveFile.DEFAULT;
        public static void Save(LanguageSaveFile save) => self.ModHelper.Storage.Save(save ?? LanguageSaveFile.DEFAULT, "languageSave.json");

    }
}