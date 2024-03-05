using UnityEngine;
using UnityEngine.UI;

public class QueryScreenFocuser : MonoBehaviour
{
    CameraState camState;
    CameraState initialCamState;
    int oldState = 0;
    InscryptionLikeCameraState camController;

    [SerializeField] Button finishButton;
    [SerializeField] TMPro.TMP_InputField queryScreen;

    private void Awake()
    {
        camState = GetComponent<CameraState>();
        camController = Camera.main.GetComponent<InscryptionLikeCameraState>();
    }

    private void OnEnable()
    {
        queryScreen.onSelect.AddListener(Focus);

        finishButton.gameObject.SetActive(true); //Just in case...
        finishButton.onClick.AddListener(UnFocus);
        finishButton.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        queryScreen.onSelect.RemoveListener(Focus);

        finishButton.gameObject.SetActive(true); //Just in case...
        finishButton.onClick.RemoveListener(UnFocus);
        finishButton.gameObject.SetActive(false);
    }

    public void Focus(string _)
    {
        if(initialCamState == null)
        {
            initialCamState = camController.GetCamState();
            oldState = initialCamState.GetState();
        }
        camController.SetCameraState(camState);
        camState.Transition(0);
        InscryptionLikeCameraState.SetBypass(true);
        
        finishButton.gameObject.SetActive(true);
    }
    public void UnFocus()
    {
        InscryptionLikeCameraState.SetBypass(false);
        camController.SetCameraState(initialCamState);
        initialCamState.Transition(oldState);
        initialCamState = null;
        
        finishButton.gameObject.SetActive(false);

        CSE.XAPI_Builder.CreateStatement_QueryConstruction(queryScreen.text);
    }
}
