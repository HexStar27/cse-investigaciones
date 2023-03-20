using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class DayCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actual,siguiente,referencia;
    [SerializeField] Animator anim;
    public string start = "Play";

    [Header("Status:")]
    [SerializeField] private bool locked = false;

    public async Task InitAnimation(int fromDay, int toDay)
    {
        if (locked) return;

        do { await Task.Delay(100); }
        while (!TempMessageController.Instancia.Terminado());

        actual.text = fromDay.ToString() + " " + GetWeekDay(fromDay);
        siguiente.text = toDay.ToString() + " " + GetWeekDay(toDay);
        if (referencia != null) referencia.text = toDay.ToString();

        if(anim != null)
        {
            anim.enabled = true;
            anim.SetTrigger(start);

            do { await Task.Delay(100); }
            while (anim.GetCurrentAnimatorStateInfo(0).IsName(start));
        }
    }

    [ContextMenu("Test")]
    public void TestFunc()
    {
        _ = InitAnimation(patata++, patata);
    }
    private static int patata = 0;

    public void DisableAnimator()
    {
        locked = false;
        actual.text = siguiente.text;
        if(anim != null) anim.enabled = false;
    }

    private static string GetWeekDay(int day)
    {
        if (day < 0) day = 0;
        day %= 7;
        return weekDay[day];
    }
    private static readonly string[] weekDay = { "Mon", "Tue", "Wed", "Tue", "Fri", "Sat", "Sun" };
}
