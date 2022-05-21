using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class DayCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actual,siguiente;
    [SerializeField] Animator anim;
    public string start = "Play";

    private bool locked = false;

    public async Task InitAnimation(int fromDay, int toDay)
    {
        if (locked) return;

        bool viaLibre;
        do{
            await Task.Delay(100);
            viaLibre = !TempMessageController.Instancia.Terminado();
        }
        while (viaLibre);


        actual.text = fromDay.ToString();
        siguiente.text = toDay.ToString();
        anim.enabled = true;
        anim.SetTrigger(start);


        do { 
            await Task.Delay(100);
        }
        while (anim.GetCurrentAnimatorStateInfo(0).IsName(start));
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
