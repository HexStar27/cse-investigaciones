using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Hexstar.UI
{
    public class SelfUpdatingSliderValue : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI tm;

        private void OnEnable()
        {
            slider.onValueChanged.AddListener(UpdateTextMesh);
        }
        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(UpdateTextMesh);
        }

        private void UpdateTextMesh(float val) { tm.text = val.ToString(); }
    }
}