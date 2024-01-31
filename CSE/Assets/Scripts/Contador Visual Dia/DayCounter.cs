using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class DayCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actual,siguiente,referencia;
    [SerializeField] Animator anim;
    public string start = "Play";

    public async Task InitAnimation(int fromDay, int toDay)
    {
        while (!TempMessageController.Instancia.Terminado()) await Task.Delay(100);

        actual.text = fromDay.ToString() + " " + GetWeekDay(fromDay);
        siguiente.text = toDay.ToString() + " " + GetWeekDay(toDay);
        if (referencia != null) referencia.text = toDay.ToString();

        if(anim != null)
        {
            anim.enabled = true;
            anim.SetTrigger(start);

            await Task.Delay(2500); 
            //La animación siempre tarda 3 segundos y medio. Termina antes para que otros sistemas puedan
            //actualizarse sin que el jugador se dé cuenta.
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
