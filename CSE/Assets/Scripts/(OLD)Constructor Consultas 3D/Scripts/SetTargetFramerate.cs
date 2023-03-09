using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
    public int targetFrameRate = 60;

    private void OnEnable()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    public void SetFPS(int maxfps)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = maxfps;
    }
}