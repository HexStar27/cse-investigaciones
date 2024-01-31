using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class QueryModeController : OptionMB
{   
    private static int manualQueryMode = 0;
    public static bool IsQueryModeOnManual() { return manualQueryMode == 1; }
    public static UnityEvent onModeChanged = new();

    [SerializeField] private Toggle m_Toggle;

    public override void Load()
    {
        manualQueryMode = PlayerPrefs.GetInt("OPTIONS/Query_Mode", 0);
        UpdateToggle();
        onModeChanged?.Invoke();
    }

    public override void Save()
    {
        PlayerPrefs.SetInt("OPTIONS/Query_Mode", manualQueryMode);
    }

    public static void ChangeQM(bool v) {
        manualQueryMode = v ? 1 : 0;
        onModeChanged?.Invoke();
    }

    public static void ForceSave()
    {
        PlayerPrefs.SetInt("OPTIONS/Query_Mode", manualQueryMode);
    }

    private void UpdateToggle() => m_Toggle.isOn = manualQueryMode > 0;


    private void OnEnable()
    {
        m_Toggle.onValueChanged.AddListener(ChangeQM);
    }
    private void OnDisable()
    {
        m_Toggle.onValueChanged.RemoveListener(ChangeQM);
    }
    private void Awake()
    {
        if (m_Toggle == null) m_Toggle = GetComponent<Toggle>();
    }
}
