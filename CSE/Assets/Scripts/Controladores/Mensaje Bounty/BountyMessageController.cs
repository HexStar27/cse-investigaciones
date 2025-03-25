using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace Hexstar.CSE
{
    public class BountyMessageController : MonoBehaviour
    {
        private static BountyMessageController Instance { get; set; }
        [SerializeField] TMPro.TextMeshProUGUI condTMesh;
        [SerializeField] TMPro.TextMeshProUGUI cantidadTMesh;
        [SerializeField] Image varIconImage;
        [SerializeField] Animator anim;
        [SerializeField] string successStateName;
        [SerializeField] string failStateName;

        [SerializeField] Sprite[] varIcons = new Sprite[0];
        AudioSource audioS;
        AudioClip clipBounty, clipNoBounty;

        private string condicionAMostrar = "";

        public static async Task SendMessage(Bounty bounty, bool win)
        {
            if (Instance == null) return;
            int iconIdx = (int)bounty.efecto;
            if (iconIdx >= Instance.varIcons.Length)
                Debug.LogError("Index of icon out of range in BountyMessageController. Index = " + iconIdx);
            Instance.varIconImage.sprite = Instance.varIcons[iconIdx];
            Instance.condTMesh.text = "";
            Instance.cantidadTMesh.text = $"{bounty.cantidad:+#;-#;+0}";
            Instance.condicionAMostrar = Bounty.Cond2Str(bounty.condicion);

            if (win)
            {
                Instance.anim.Play(Instance.successStateName);
                await Task.Delay(3034); // anim = 2833, extra = 200
            }
            else
            {
                Instance.anim.Play(Instance.failStateName);
                await Task.Delay(2867); // anim = 2666, extra = 200
            }
        }

        [ContextMenu("Test Success")]
        private void TestSuccess()
        {
            anim.Play(successStateName);
        }
        [ContextMenu("Test Failure")]
        private void TestFailure()
        {
            anim.Play(failStateName);
        }

        /// <summary>
        /// Siempre va a tardar 0.5s (aprox) en colocar las letras.
        /// </summary>
        public IEnumerator ColocarLetrasConsecutivas()
        {
            float n = condicionAMostrar.Length;
            WaitForSeconds delay = new(0.5f / n); //Tiene 0.5 segundos para colocar todas las letras
            bool is_escape = false;
            for (int i = 0; i < n; i++)
            {
                char c = condicionAMostrar[i];
                condTMesh.text += c;
                
                if (c == '<') is_escape = true;
                else if (c == '>') is_escape = false;
                
                if (!is_escape) yield return delay;
            }
        }

        public void PlayBountySFX() => audioS.PlayOneShot(clipBounty);
        public void PlayNoBountySFX() => audioS.PlayOneShot(clipNoBounty);
        

        private void Awake()
        {
            audioS = GetComponent<AudioSource>();
            clipBounty = Resources.Load<AudioClip>("Audio/SFX/SFX_Bounty");
            clipNoBounty = Resources.Load<AudioClip>("Audio/SFX/SFX_NO_Bounty");
            Instance = this;
        }
    }
}