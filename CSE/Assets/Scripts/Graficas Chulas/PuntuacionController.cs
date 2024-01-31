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
    public static bool puntuacionEnPantalla = false;

    [SerializeField] RectTransform conjunto;

    [SerializeField] GM2PC gPuntuacion;
    [SerializeField] GM2PC gTiempo;
    [SerializeField] GM2PC gConsulta;

    [SerializeField] TextMeshProUGUI puntText;
    public float duracionAnim = 3f;
    [SerializeField] Animator anim;
    [SerializeField] string mostrarPuntAnim = "Show";
    [SerializeField] string esconderPuntAnim = "Hide";
    [SerializeField] AudioClip clipPuntuacion;
    [SerializeField] AudioSource speaker;

    int puntuacion = 0;

    static int[] gp, gt, gc;

    private void Awake()
    {
        Instance = this;
    }

    public void MostrarPuntuacion() { StartCoroutine(MostrarPunt(puntuacion)); }

    public void PrepararGraficaP(int[] valores) { gPuntuacion.Setup(valores, OperacionesGameplay.s_lastScore); }
    public void PrepararGraficaT(int[] valores) { gTiempo.Setup(valores, PuzzleManager.UltimoTiempoEmpleado); }
    public void PrepararGraficaC(int[] valores) { gConsulta.Setup(valores,PuzzleManager.ConsultasRealizadasActuales); }

    private IEnumerator MostrarPunt(int objetivo)
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float oldTime = Time.time;
        float t = 0;
        int frame = 0;

        while (t < duracionAnim)
        {
            t = Time.time - oldTime;
            float f = Serp01(t / duracionAnim);
            puntText.text = Mathf.RoundToInt(objetivo*f).ToString();
            if (frame % 5 == 0 && clipPuntuacion != null)
            {
                speaker.pitch = (f*0.5f)+0.5f;
                speaker.PlayOneShot(clipPuntuacion);
            }
            frame++;
            yield return wait;
        }
        puntText.text = objetivo.ToString();
    }

    public async Task Extraer()
    {
        int tiempoEmpleado = Mathf.FloorToInt(PuzzleManager.GetSetTiempoEmpleado());

        WWWForm form = new WWWForm();
        int caso = PuzzleManager.GetIdCasoActivo();
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
            InscryptionLikeCameraState.SetBypass(true);
            Boton3D.globalStop = true;
            puntuacionEnPantalla = true;
        }
    }

    public void QuitarPuntuacion()
    {
        anim.Play(esconderPuntAnim);
    }

    public void HideMeshes()
    {
        gPuntuacion.Hide();
        gTiempo.Hide();
        gConsulta.Hide();
    }

    public void LetCameraGo()
    {
        puntuacionEnPantalla = false;
        Boton3D.globalStop = false;
        InscryptionLikeCameraState.SetBypass(false);
    }
}
