using UnityEngine;

namespace CSE {
    public class DesktopZone_xAPI_setup : MonoBehaviour
    {
        [SerializeField] QuickSwitch qs;
        private void OnEnable()
        {
            qs.onState1.AddListener(Setup2Output);
            qs.onState0.AddListener(Setup2Input);
        }
        private void OnDisable()
        {
            qs.onState1.RemoveListener(Setup2Output);
            qs.onState0.RemoveListener(Setup2Input);
        }

        private void Setup2Output() => XAPI_Builder.CreateStatement_SwitchDesktopZone(true);
        private void Setup2Input() => XAPI_Builder.CreateStatement_SwitchDesktopZone(false);
    }
}