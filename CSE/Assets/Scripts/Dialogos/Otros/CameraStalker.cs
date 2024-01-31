using UnityEngine;
using UnityEngine.Events;

public class CameraStalker : MonoBehaviour
{
    [SerializeField] InscryptionLikeCameraState cam;
    public int stateToWaitFor = 0;

    public UnityEvent onCameraReady = new();

    private void FixedUpdate()
    {
        if (cam.GetCamState().GetState() == stateToWaitFor)
        {
            onCameraReady?.Invoke();
            gameObject.SetActive(false);
        }
    }
    public void SetTarget(int target)
    {
        stateToWaitFor = target;
    }
}
