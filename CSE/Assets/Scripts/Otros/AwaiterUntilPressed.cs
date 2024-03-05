using UnityEngine;

public class AwaiterUntilPressed : MonoBehaviour
{
    [SerializeField] private Transform popup;

    public void Press()
    {
        popup.gameObject.SetActive(false);
        GameplayCycle.PauseGameplayCycle(false, "Popup");
    }


    public void ActivarPopUp()
    {
        popup.gameObject.SetActive(true);
        GameplayCycle.PauseGameplayCycle(true, "Popup");
    }
}
