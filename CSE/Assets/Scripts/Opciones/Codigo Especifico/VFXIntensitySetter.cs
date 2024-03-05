using UnityEngine;
using UnityEngine.Rendering;

public class VFXIntensitySetter : MonoBehaviour
{
    Volume volume;
    private void Awake()
    {
        volume = GetComponent<Volume>();
    }
    private void OnEnable()
    {
        VFXIntensityController.onOptionChanged.AddListener(UpdateVolume);
    }
    private void OnDisable()
    {
        VFXIntensityController.onOptionChanged.RemoveListener(UpdateVolume);
    }
    private void UpdateVolume(float val)
    {
        volume.weight = val;
    }
}
