using UnityEditor;
using UnityEngine;

namespace Lovatto.Localization
{
    public class bl_CSVLanguageFileHandler : EditorWindow
    {
        public bl_LanguageTexts language;

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            titleContent = new GUIContent("CSV Handler");
        }

        public static void HandleLanguage(bl_LanguageInfo lang)
        {
            var window = GetWindow<bl_CSVLanguageFileHandler>();
            window.language = lang.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.Label("CSV Language File Handler", EditorStyles.boldLabel);
            GUILayout.Space(10);

            language = EditorGUILayout.ObjectField("Language", language, typeof(bl_LanguageTexts), false) as bl_LanguageTexts;
            GUI.enabled = language != null;
            if (GUILayout.Button("Export Language to CSV file"))
            {
                Export();
            }
            GUILayout.Space(10);
            GUI.enabled = true;
            if (GUILayout.Button("Import CSV file to Language"))
            {
                Import();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void Export()
        {
            if(language == null) { return; }
            var sourceLang = bl_Localization.Instance.DefaultLanguage;
            if (sourceLang == null)
            {
                Debug.LogError("Default language not found, please set a default language in Localization Manager.");
                return;
            }

            string path = EditorUtility.SaveFilePanel("Save CSV File", "", $"{sourceLang.LanguageName}-to-{language.LanguageName}", "csv");
            if (string.IsNullOrEmpty(path)) { return; }
         
            string csv = $"Source Language: {sourceLang.LanguageName},Target Language: {language.LanguageName}\n";
            csv += $"Language Code,{(int)language.LanguageCode}\n";
            csv += $"Plural Letter,{language.PlurarLetter}\n";
            string iconPath = "";
            if (language.LanguageIcon != null)
            {
                iconPath = AssetDatabase.GetAssetPath(language.LanguageIcon);
            }
            csv += $"Language Icon,{iconPath}\n";
            csv += "--------,--------\n";

            var defaultTextList = sourceLang.Text.Data;
            var textList = language.Data;
            for (int i = 0; i < textList.Length; i++)
            {
                string line = $"\"{defaultTextList[i].Text}\",\"{textList[i].Text}\"\n";
                csv += line;
            }

            System.IO.File.WriteAllText(path, csv);
            AssetDatabase.Refresh();
            Debug.Log($"{language.LanguageName} CSV file saved at: {path}");
        }

        void Import()
        {
            var sourceLang = bl_Localization.Instance.DefaultLanguage;
            if (sourceLang == null)
            {
                Debug.LogError("Default language not found, please set a default language in Localization Manager.");
                return;
            }

            // get CSV path
            string path = EditorUtility.OpenFilePanel("Open CSV File", "", "csv");
            if (string.IsNullOrEmpty(path)) { return; }
            // read CSV file
            string[] lines = System.IO.File.ReadAllLines(path);
            if (lines.Length <= 0) { return; }
            // get source language
            string languageName = lines[0].Split(',')[1].Replace("Target Language: ","").Trim();
            string languageCode = lines[1].Split(',')[1].Trim();
            string pluralLetter = lines[2].Split(',')[1];
            string iconPath = lines[3].Split(',')[1];
            int textOffset = 5;

            bl_LanguageTexts langText = ScriptableObject.CreateInstance<bl_LanguageTexts>();
            langText.LanguageName = languageName;
            langText.LanguageCode = (LocalizationLanguageCode)int.Parse(languageCode);
            langText.PlurarLetter = pluralLetter;
            if (!string.IsNullOrEmpty(iconPath))
            {
                langText.LanguageIcon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            }
            langText.Data = new bl_LanguageTexts.TextData[lines.Length - textOffset];

            // get source language
            for (int i = textOffset; i < lines.Length; i++)
            {
                string text = lines[i].Split(',')[1];
                // remove quotes from start and end
                text = text.Substring(1, text.Length - 2);

                langText.Data[i - textOffset] = new bl_LanguageTexts.TextData();
                langText.Data[i - textOffset].Text = text;
                if((i - textOffset) >= sourceLang.Text.Data.Length)
                {
                    Debug.LogWarning($"The {languageName} language has more texts than the default language, some texts will require to manually set the Key in the editor.");
                    continue;
                }
                langText.Data[i - textOffset].StringID = sourceLang.Text.Data[i - textOffset].StringID;
            }

            // save language
            string savePath = EditorUtility.SaveFilePanel("Save Language", Application.dataPath, languageName, "asset");
            if (string.IsNullOrEmpty(savePath)) { return; }

            // make path relative to Assets folder
            savePath = "Assets" + savePath.Replace(Application.dataPath, "");

            AssetDatabase.CreateAsset(langText, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //EditorGUIUtility.PingObject(langText);
            Debug.Log($"{languageName} language saved at: {savePath}");

        }
    }
}