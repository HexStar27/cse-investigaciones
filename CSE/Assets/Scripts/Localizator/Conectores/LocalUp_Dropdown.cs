using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CSE.Local
{
    public class LocalUp_Dropdown : LocalizationUpdater
    {
        [SerializeField] TMP_Dropdown _dd;

        private void Awake()
        {
            if (_dd == null) _dd = GetComponent<TMP_Dropdown>();
            updateAction = TranslateString;
        }
        private void TranslateString()
        {
            _dd.ClearOptions();
            _dd.AddOptions(new List<string>(Localizator.GetString(key).Split(";")));
        }
    }
}
