using UnityEngine;

public class OpctionsHooker : MonoBehaviour
{
    public void ToogleOptionsMenu(bool value)
    {
        OptionsMenuRoot.Instance().transform.GetChild(0).gameObject.SetActive(value);
    }
}
