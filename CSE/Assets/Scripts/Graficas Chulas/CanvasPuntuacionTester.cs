using UnityEngine;

namespace CSE.Testers
{
    public class CanvasPuntuacionTester : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] GM2PC g_puntuacion;
        [SerializeField] GM2PC g_tiempo;
        [SerializeField] GM2PC g_consultasUsadas;
        [SerializeField] RectTransform grupoDeCanvas;

        static readonly string mostrarPuntAnim = "Show";

        private void Start()
        {
            grupoDeCanvas.gameObject.SetActive(true);

            int[] datosDeEjemplo = { 5,8,6,10,12,10,7,2,3,2,3 };
            g_puntuacion.Setup(datosDeEjemplo,2);
            g_tiempo.Setup(datosDeEjemplo, 7);
            g_consultasUsadas.Setup(datosDeEjemplo,12);

            anim.Play(mostrarPuntAnim);
        }
    }
}