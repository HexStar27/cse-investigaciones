using UnityEngine;

public class QuickSwitch : MonoBehaviour
{
    public Boton3D.Evento onState0, onState1;
    public Boton3D m_Boton3d;
    private bool stateSwaped;
    private void OnEnable()
    {
        if (m_Boton3d == null)
        {
            if (!TryGetComponent(out m_Boton3d))
            {
                Debug.LogError("El QuickSwitch no ha encontrado un boton3d en su mismo gameobject.");
                return;
            }
        }

        m_Boton3d.onClick.AddListener(Swap);
    }
    private void OnDisable()
    {
        if (m_Boton3d != null) m_Boton3d.onClick.RemoveListener(Swap);
    }

    private void Swap()
    {
        stateSwaped = !stateSwaped;
        if (stateSwaped) onState1?.Invoke();
        else onState0?.Invoke();
    }
}
