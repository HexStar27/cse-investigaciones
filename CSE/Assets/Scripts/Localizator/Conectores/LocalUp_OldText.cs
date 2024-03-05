using UnityEngine;
using UnityEngine.UI;

namespace CSE.Local
{
    public class LocalUp_OldText : LocalizationUpdater
    {
        [SerializeField] Text _tm;
        private void Awake()
        {
            if (_tm == null) _tm = GetComponent<Text>();
            updateAction = TranslateString;
        }
        private void TranslateString() => _tm.text = Localizator.GetString(key);
    }
}