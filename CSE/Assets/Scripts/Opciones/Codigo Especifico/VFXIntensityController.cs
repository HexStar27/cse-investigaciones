using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Se encarga de establecer la intensidad de los efectos visuales como el modo DANGER, el post-procesado, etc.
/// </summary>
public class VFXIntensityController : OptionMB, ISliderValue
{
    public class FLOATEVENT : UnityEvent<float> { }
    public static FLOATEVENT onOptionChanged = new FLOATEVENT();
    [SerializeField] VFXIntensityConfig config;
    [SerializeField] Slider slider;
    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }
    private void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateValue);
    }
    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateValue);
    }
    private void UpdateValue(float value)
    {
        config.postProcesingBlend = value;
        onOptionChanged?.Invoke(value);
    }

    public override void Load()
    {
        float val = PlayerPrefs.GetFloat("OPTIONS/VFX_Intensity", 1);
        config.postProcesingBlend = val;
        slider.value = val;
    }

    public override void Save()
    {
        PlayerPrefs.SetFloat("OPTIONS/VFX_Intensity", config.postProcesingBlend);
    }

    public Slider GetSlider()
    {
        return slider;
    }
}
