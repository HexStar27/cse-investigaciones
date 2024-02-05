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
                await Task.Delay(2934); // anim = 2833, extra = 100
            }
            else
            {
                Instance.anim.Play(Instance.failStateName);
                await Task.Delay(2767); // anim = 2666, extra = 100
            }
        }

        /// <summary>
        /// Siempre va a tardar 0.5s (aprox) en colocar las letras.
        /// </summary>
        public IEnumerator ColocarLetrasConsecutivas()
        {
            float n = condicionAMostrar.Length;
            WaitForSeconds delay = new(0.5f / n); //Tiene 0.5 segundos para colocar todas las letras
            for (int i = 0; i < n; i++)
            {
                condTMesh.text += condicionAMostrar[i];
                yield return delay;
            }
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}