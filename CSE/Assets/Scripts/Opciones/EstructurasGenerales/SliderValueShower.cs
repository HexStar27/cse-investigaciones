using UnityEngine;

public class SliderValueShower : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public ISliderValue sliderController;

    private void Awake()
    {
        sliderController = GetComponent<ISliderValue>();
    }

    private void OnEnable()
    {
        sliderController.GetSlider().onValueChanged.AddListener(UpdateValue);
        UpdateValue(sliderController.GetSlider().value);
    }

    private void OnDisable()
    {
        sliderController.GetSlider().onValueChanged.RemoveListener(UpdateValue);
    }

    public void UpdateValue(float val) { text.text = ((int)(val * 100)).ToString() + "%"; }
}
