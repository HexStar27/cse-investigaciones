using System.Collections;
using UnityEngine;
using TMPro;

public class DayCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actual,siguiente;
    [SerializeField] Animator anim;
    public string start = "Play";

    private bool locked = false;

    public void InitAnimation(int fromDay, int toDay)
    {
        if (locked) return;
        actual.text = fromDay.ToString();
        siguiente.text = toDay.ToString();
        anim.enabled = true;
        anim.SetTrigger(start);
    }

    public void TestFunc()
    {
        if (locked) return;
        locked = true;
        actual.text = patata.ToString();
        siguiente.text = (++patata).ToString();
        anim.enabled = true;
        anim.SetTrigger(start);
    }
    private static int patata = 0;

    public void DisableAnimator()
    {
        anim.enabled = false;
        actual.text = siguiente.text;
        locked = false;
    }
}
