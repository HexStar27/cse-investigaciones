using UnityEngine;
using UnityEngine.Events;

public class CameraStalker : MonoBehaviour
{
    [SerializeField] InscryptionLikeCameraState cam;
    public int stateToWaitFor = 0;
    public bool inverse = false;
    public bool closeOnReady = true;

    public UnityEvent onCameraReady = new();

    private void FixedUpdate()
    {
        bool detected = cam.GetCamState().GetState() == stateToWaitFor;
        if (detected ^ inverse)
        {
            onCameraReady?.Invoke();
            if (closeOnReady) gameObject.SetActive(false);
        }
    }
    public void SetTarget(int target)
    {
        stateToWaitFor = target;
    }

    public bool CorrectState() => (cam.GetCamState().GetState() == stateToWaitFor) ^ inverse;
}
