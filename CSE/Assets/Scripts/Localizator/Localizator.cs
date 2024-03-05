using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace CSE.Local
{
    public static class Localizator
    {
        private readonly static Dictionary<string, Dictionary<string, string>> _translation = new();
        private readonly static List<string> _availableLanguages = new();
        private readonly static string path = Path.Combine(Application.streamingAssetsPath, "Localization", "localization.json");
        private static int _languageIdx = 0;

        private static bool _debug = false;

        public static UnityEvent onLanguageChanged = new();

        public static void ReadTranslations()
        {
            string content = File.ReadAllText(path);
            var json = JSON.Parse(content);
            var entriesEnumerator = json.Keys;

            List<string> entryNames = new();
            while (entriesEnumerator.MoveNext()) entryNames.Add(entriesEnumerator.Current);

            _availableLanguages.Clear();
            _translation.Clear();

            int n = entryNames.Count;
            if (_debug) Debug.Log("Entries found: " + n);
            for (int i = 0; i < n; i++)
            {
                string currentEntryName = entryNames[i];
                if (_debug) Debug.Log(currentEntryName);
                Dictionary<string, string> entryContent = new();
                var langEnumerable = json[currentEntryName].Keys;
                while (langEnumerable.MoveNext())
                {
                    string langName = langEnumerable.Current;
                    if (!_availableLanguages.Contains(langName)) _availableLanguages.Add(langName);
                    string translation = json[currentEntryName][langName];
                    entryContent.Add(langName, translation);
                    if (_debug) Debug.Log("- " + langName + ": " + translation);
                }
                _translation.Add(currentEntryName, entryContent);
            }

            _languageIdx = PlayerPrefs.GetInt("lang", 0);
        }
        public static void SetLanguage(string language)
        {
            int idx = Math.Max(_availableLanguages.IndexOf(language), 0);
            bool diff = _languageIdx != idx;
            _languageIdx = idx;
            if (diff)
            {
                onLanguageChanged?.Invoke();
                PlayerPrefs.SetInt("lang", _languageIdx);
                if (_debug) Debug.Log("Language set to ->" + _availableLanguages[_languageIdx]);
            }
        }
        public static void SetLanguage(int idx)
        {
            if (idx < 0 || idx >= _availableLanguages.Count) return;
            bool diff = _languageIdx != idx;
            _languageIdx = idx;
            if (diff)
            {
                onLanguageChanged?.Invoke();
                PlayerPrefs.SetInt("lang", _languageIdx);
                if (_debug) Debug.Log("Language set to ->" + _availableLanguages[_languageIdx]);
            }
        }
        public static List<string> Languages()
        {
            return _availableLanguages;
        }
        public static string GetString(string code)
        {
            if (_translation.Count == 0)
            {
                Debug.Log("Tried to use Localizator without it being loaded, trying to load now.");
                ReadTranslations();
            }
            if (!_translation.ContainsKey(code))
            {
                Debug.LogError("Entry "+code+" not found in the Localizator");
                return "";
            }
            var entry = _translation[code];
            if (entry.TryGetValue(_availableLanguages[_languageIdx], out string v)) return v;
            else return entry[_availableLanguages[0]];
        }
    }
}