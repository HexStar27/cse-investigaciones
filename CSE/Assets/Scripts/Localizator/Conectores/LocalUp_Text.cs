using TMPro;
using UnityEngine;

namespace CSE.Local
{
    public class LocalUp_Text : LocalizationUpdater
    {
        [SerializeField] TextMeshProUGUI _tm;

        private void Awake()
        {
            if(_tm == null) _tm = GetComponent<TextMeshProUGUI>();
            updateAction = TranslateString;
        }
        private void TranslateString() => _tm.SetText(Localizator.GetString(key));
    }
}