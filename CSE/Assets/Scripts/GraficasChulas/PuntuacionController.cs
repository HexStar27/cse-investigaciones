using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using Hexstar;
using SimpleJSON;
using System.Collections.Generic;

public class PuntuacionController : MonoBehaviour
{
    private static PuntuacionController instance;
    public static PuntuacionController Instance { get { return instance; } private set { instance = value; } }

    [SerializeField] RectTransform conjunto;

    [SerializeField] GM2PC gPuntuacion;
    [SerializeField] GM2PC gTiempo;
    [SerializeField] GM2PC gConsulta;

    [SerializeField] TextMeshProUGUI puntText;
    public float duracionAnim = 2f;
    [SerializeField] Animator anim;
    [SerializeField] string mostrarPuntAnim = "Show";
    [SerializeField] AudioClip clipPuntuacion;
    [SerializeField] AudioSource speaker;

    int puntuacion = 0;

    static int[] gp, gt, gc;

    private void Awake()
    {
        Instance = this;
    }

    public void MostrarPuntuacion() { StartCoroutine(MostrarPunt(puntuacion)); }

    public void PrepararGraficaP(int[] valores) { gPuntuacion.Setup(valores); }
    public void PrepararGraficaT(int[] valores) { gTiempo.Setup(valores); }
    public void PrepararGraficaC(int[] valores) { gConsulta.Setup(valores); }

    private IEnumerator MostrarPunt(int objetivo)
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float oldTime = Time.time;
        float t = 0;

        while (t < duracionAnim)
        {
            t = Time.time - oldTime;
            puntText.text = Mathf.RoundToInt(objetivo*Serp01(t/duracionAnim)).ToString();
            if (clipPuntuacion != null) speaker.PlayOneShot(clipPuntuacion);
            yield return wait;
        }
        puntText.text = objetivo.ToString();
    }

    public async Task Extraer()
    {
        int tiempoEmpleado = Mathf.FloorToInt(PuzzleManager.GetTiempoEmpleado());

        WWWForm form = new WWWForm();
        int caso = PuzzleManager.GetCasoActivo().id;
        form.AddField("dif", -1);
        form.AddField("tipo", 5);
        form.AddField("caso", caso);
        await ConexionHandler.APost(ConexionHandler.baseUrl + "score", form);

        string json = ConexionHandler.ExtraerJson(ConexionHandler.download);
        if (json == "{}") Debug.LogError("Ha habido un error en el servidor al calcular la puntuación :(");
        else
        {
            //print(ConexionHandler.download);
            List<int> s = new List<int>();
            List<int> q = new List<int>();
            List<int> t = new List<int>();
            JSONNode jNodo = JSON.Parse(ConexionHandler.download)["res"];
            var array = jNodo.AsArray;
            int n = array.Count;
            for (int i = 0; i < n; i++)
            {
                s.Add(array[i]["S"]);
                q.Add(array[i]["Q"]);
                t.Add(array[i]["T"]);
            }
            gp = s.ToArray();
            gc = q.ToArray();
            gt = t.ToArray();
        }
    }

    private float Serp01(float t)
    {
        return (1 + Mathf.Sin(Mathf.PI * (t - .5f))) * .5f;
    }

    //Pendiente más pronunciada que el Serp01 original
    private float Serp01Pow(float t)
    {
        return (1 + Mathf.Pow(Mathf.Sin(Mathf.PI * (t - .5f)), 0.6f)) * .5f;
    }

    public static async Task PresentarPuntuaciones(int puntuacionJugador)
    {
        if (Instance == null) Debug.LogError("La instancia del 'PuntuacionController' es null");
        else
        {
            //1º Pedir info al server
            Instance.puntuacion = puntuacionJugador;
            Instance.conjunto.gameObject.SetActive(true);
            await Instance.Extraer();

            //2º Poner los valores en las gráficas
            Instance.PrepararGraficaP(gp);
            Instance.PrepararGraficaT(gt);
            Instance.PrepararGraficaC(gc);

            //3º Empezar la animación
            Instance.anim.Play(Instance.mostrarPuntAnim);
            InscryptionLikeCameraState.bypass = true;
        }
    }

    public void LetCameraGo()
    {
        InscryptionLikeCameraState.bypass = false;
    }
}
