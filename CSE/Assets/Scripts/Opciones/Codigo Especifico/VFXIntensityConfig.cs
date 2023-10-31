using UnityEngine;

[CreateAssetMenu(fileName = "VFX Intensity Configuration", menuName = "CSE/Configuration/VFX")]
public class VFXIntensityConfig : ScriptableObject
{
    [Range(0, 1)]
    public float postProcesingBlend = 1;
}
