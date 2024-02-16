using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace CSE.Local
{
    public class LocalizatorSetup : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown languageSelector;
        private static List<Sprite> flags = new();

        public bool skipLoad = false;

        void Awake()
        {
            if(!skipLoad)
            {
                Localizator.ReadTranslations();
                LoadFlags();
            }
            var viewportRect = languageSelector.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
            viewportRect.sizeDelta = new(viewportRect.sizeDelta.x, 100 * flags.Count);
            languageSelector.ClearOptions();
            languageSelector.AddOptions(flags);
            languageSelector.onValueChanged.AddListener((i) => Localizator.SetLanguage(i));
            languageSelector.SetValueWithoutNotify(PlayerPrefs.GetInt("lang", 0));
        }

        private void LoadFlags()
        {
            flags.Clear();
            var ll = Localizator.Languages();
            foreach (var lang in ll)
            {
                string path = Path.Combine(Application.streamingAssetsPath, "Localization", lang + ".png");
                Texture2D tex = new(1, 1);
                if (File.Exists(path))
                {
                    var bytes = File.ReadAllBytes(path);
                    tex.LoadImage(bytes);
                }
                flags.Add(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 64));
            }
        }
    }
}