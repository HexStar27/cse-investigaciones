using UnityEngine;
using UnityEngine.Events;

namespace CSE.Local
{
    public class LocalizationUpdater : MonoBehaviour
    {
        public string key = ".something";
        protected UnityAction updateAction;
        private void OnEnable()
        {
            Localizator.onLanguageChanged.AddListener(updateAction);
        }
        private void OnDisable()
        {
            Localizator.onLanguageChanged.RemoveListener(updateAction);
        }
        private void Start()
        {
            updateAction();
        }
    }
}