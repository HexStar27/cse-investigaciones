using UnityEngine;

public class QueryInputModeModifier : MonoBehaviour
{
    public Transform modoBloques;
    public Transform modoManual;
    public AperturaTableta tableta;

    public void SetManualMode(bool manual)
    {
        if(manual)
        {
            modoBloques.gameObject.SetActive(false);
            tableta.ForceClose();
            tableta.gameObject.SetActive(false);
            modoManual.gameObject.SetActive(true);
        }
        else
        {
            modoBloques.gameObject.SetActive(true);
            tableta.gameObject.SetActive(true);
            modoManual.gameObject.SetActive(false);
        }
    }

    private void UpdateFromController() => SetManualMode(QueryModeController.IsQueryModeOnManual());

    private void Start()
    {
        UpdateFromController();
    }
    private void OnEnable()
    {
        QueryModeController.onModeChanged.AddListener(UpdateFromController);
    }
    private void OnDisable()
    {
        QueryModeController.onModeChanged.RemoveListener(UpdateFromController);
    }
}
