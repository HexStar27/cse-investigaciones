using UnityEngine;

public class QualityController : OptionMB
{
    [SerializeField] TMPro.TMP_Dropdown dropdown;

    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(ApplyQuality);
    }
    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(ApplyQuality);
    }

    public override void Load()
    {
        int val = PlayerPrefs.GetInt("OPTIONS/Quality", 1);
        ApplyQuality(val);
        dropdown.value = val;
    }

    public override void Save()
    {
        PlayerPrefs.SetInt("OPTIONS/Quality", QualitySettings.GetQualityLevel());
    }

    public void ApplyQuality(int l)
    {
        QualitySettings.SetQualityLevel(l, true);
    }
}
