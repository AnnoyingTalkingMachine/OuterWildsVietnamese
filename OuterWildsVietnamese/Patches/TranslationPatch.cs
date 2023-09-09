using HarmonyLib;
using System.Reflection;
using System.Xml;
using System;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.Android;

namespace OuterWildsVietnamese
{
    [HarmonyPatch]
    public static class TranslationPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.SetLanguage))]
        public static bool SetLanguage(
            TextTranslation.Language lang,
            TextTranslation __instance,
            ref TextTranslation.Language ___m_language,
            ref TextTranslation.TranslationTable ___m_table)
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(lang)) return true;

            //Luminescence090823: This very line of code is causing the error. Trying to figure out why.
            //___m_language = lang;
            ___m_table = null;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(ReadAndRemoveByteOrderMarkFromPath(OuterWildsVietnamese.translationPath));

                var translationTableNode = xmlDoc.SelectSingleNode("TranslationTable_XML");
                if (translationTableNode == null)
                {
                    OuterWildsVietnamese.Log("TranslationTable_XML could not be found in translation file.");
                    ___m_language = TextTranslation.Language.UNKNOWN;
                    return true;
                }

                var translationTable_XML = new TextTranslation.TranslationTable_XML();

                // Add regular text to the table
                foreach (XmlNode node in translationTableNode.SelectNodes("entry"))
                {
                    var key = node.SelectSingleNode("key").InnerText;
                    var value = node.SelectSingleNode("value").InnerText;

                    translationTable_XML.table.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add ship log entries
                foreach (XmlNode node in translationTableNode.SelectSingleNode("table_shipLog").SelectNodes("TranslationTableEntry"))
                {
                    var key = node.SelectSingleNode("key").InnerText;
                    var value = node.SelectSingleNode("value").InnerText;

                    translationTable_XML.table_shipLog.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add UI
                foreach (XmlNode node in translationTableNode.SelectSingleNode("table_ui").SelectNodes("TranslationTableEntryUI"))
                {
                    // Keys for UI are all ints
                    var key = int.Parse(node.SelectSingleNode("key").InnerText);
                    var value = node.SelectSingleNode("value").InnerText;

                    translationTable_XML.table_ui.Add(new TextTranslation.TranslationTableEntryUI(key, value));
                }

                ___m_table = new TextTranslation.TranslationTable(translationTable_XML);

                //xen: Goofy stuff to envoke event
                //Luminescence010723: I don't even know what this does, but it seems important enough so it stays.
                //Luminescence020723: This piece of black box is causing error. Removing it brings back the menu, but disables language change entirely. Great.
                //Luminescence020723: Maybe the real problem lies in SliderPatch, or TextTranslation.Language enum. I'll leave the investigation for tomorrow.
                //Luminescence020723: Just realized I've been commenting like what the Nomai usually do. Nice.
                //Luminescence240723: Finally quit my job to get back to this. The "leave the investigation for tomorrow" part turned out to be 3 weeks later.
                var onLanguageChanged = (MulticastDelegate)__instance.GetType().GetField("OnLanguageChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
                if (onLanguageChanged != null)
                {
                    onLanguageChanged.DynamicInvoke();
                }
            }
            catch (Exception ex)
            {
                OuterWildsVietnamese.Log($"Couldn't load Vietnamese translation: {ex.Message}{ex.StackTrace}");
                ___m_language = TextTranslation.Language.UNKNOWN;
                return true;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation._Translate))]
        public static bool _Translate(
            string key,
            TextTranslation __instance,
            ref string __result,
            TextTranslation.Language ___m_language,
            TextTranslation.TranslationTable ___m_table)
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(___m_language)) return true;

            if (___m_table == null)
            {
                OuterWildsVietnamese.Log("TextTranslation has not been initialized.");
                __result = key;
                return false;
            }

            string text = ___m_table.Get(key);
            if (text == null)
            {
                OuterWildsVietnamese.Log("String \"" + key + "\" not found in table.");
                __result = key;
                return false;
            }
            text = text.Replace("\\\\n", "\n");
            __result = text;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation._Translate_ShipLog))]
        public static bool _Translate_ShipLog(
            string key,
            TextTranslation __instance,
            ref string __result,
            TextTranslation.Language ___m_language,
            TextTranslation.TranslationTable ___m_table)
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(___m_language)) return true;

            if (___m_table == null)
            {
                OuterWildsVietnamese.Log("TextTranslation has not been initialized.");
                __result = key;
                return false;
            }

            string text = ___m_table.GetShipLog(key);
            if (text == null)
            {
                OuterWildsVietnamese.Log("String \"" + key + "\" not found in ShipLog table.");
                __result = key;
                return false;
            }
            text = text.Replace("\\\\n", "\n");
            __result = text;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation._Translate_UI))]
        public static bool _Translate_UI(
            int key,
            TextTranslation __instance,
            ref string __result,
            TextTranslation.Language ___m_language,
            TextTranslation.TranslationTable ___m_table)
        {
            if (OuterWildsVietnamese.IsVanillaLanguage(___m_language)) return true;

            if (___m_table == null)
            {
                OuterWildsVietnamese.Log("TextTranslation has not been initialized.");
                __result = key.ToString();
                return false;
            }

            string text = ___m_table.Get_UI(key);
            if (text == null)
            {
                OuterWildsVietnamese.Log("String \"" + key + "\" not found in UI table.");
                __result = key.ToString();
                return false;
            }
            text = text.Replace("\\\\n", "\n");
            __result = text;
            return false;
        }


        private static string ReadAndRemoveByteOrderMarkFromPath(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            byte[] preamble1 = Encoding.UTF8.GetPreamble();
            byte[] preamble2 = Encoding.Unicode.GetPreamble();
            byte[] preamble3 = Encoding.BigEndianUnicode.GetPreamble();
            if (bytes.StartsWith(preamble1))
                return Encoding.UTF8.GetString(bytes, preamble1.Length, bytes.Length - preamble1.Length);
            if (bytes.StartsWith(preamble2))
                return Encoding.Unicode.GetString(bytes, preamble2.Length, bytes.Length - preamble2.Length);
            return bytes.StartsWith(preamble3) ? Encoding.BigEndianUnicode.GetString(bytes, preamble3.Length, bytes.Length - preamble3.Length) : Encoding.UTF8.GetString(bytes);
        }
    }
}
