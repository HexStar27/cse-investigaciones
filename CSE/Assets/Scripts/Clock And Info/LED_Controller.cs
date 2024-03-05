using System.Threading.Tasks;
using UnityEngine;

public class LED_Controller : MonoBehaviour
{
    public static LED_Controller Instance { get; private set; }

    [SerializeField] MeshRenderer _r;
    [SerializeField] AudioClip _LEDfx_1;
    [SerializeField] AudioClip _LEDfx_2;
    [SerializeField] AudioSource _as;

    Material _ledMat;

    readonly Vector4 gray = new(0.5943396f, 0.5943396f, 0.5943396f, 1f);
    readonly Vector4 red = new Vector4(32f,0f,0f,1f);
    readonly Vector4 green = new Vector4(0f,16f,0f,1f);
    readonly int delay = 2000;

    [ContextMenu("Test LED")]
    public async Task TurnRed()
    {
        _ledMat.SetVector("_EmissionColor", red);
        PlaySFX(false);
        await Task.Delay(delay);
        TurnOff();
    }
    public async Task TurnGreen()
    {
        _ledMat.SetVector("_EmissionColor", green);
        PlaySFX(true);
        await Task.Delay(delay);
        TurnOff();
    }
    public void TurnOff()
    {
        _ledMat.SetVector("_EmissionColor", gray);
    }

    private void PlaySFX(bool ganado)
    {
        if (_as == null) return;
        if(ganado)
        {
            if (_LEDfx_1 != null) _as.PlayOneShot(_LEDfx_1);
        }
        else
        {
            if (_LEDfx_2 != null) _as.PlayOneShot(_LEDfx_2);
        }
    }

    private void Awake()
    {
        Instance = this;
        _ledMat = _r.sharedMaterials[1];
        _LEDfx_1 = Resources.Load<AudioClip>("Audio/Sfx/led-nice2");
        _LEDfx_2 = Resources.Load<AudioClip>("Audio/Sfx/led-nice");
        TurnOff();
    }
}
