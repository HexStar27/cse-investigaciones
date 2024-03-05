using UnityEngine;

namespace Hexstar.Dialogue
{
    public class Flechita : MonoBehaviour
    {
        [SerializeField] ControladorCajaDialogos ccd;
        [SerializeField] Transform flechita;
        private void OnEnable()
        {
            ccd.onFinishPlacement.AddListener(Show);
        }
        private void OnDisable()
        {
            ccd.onFinishPlacement.RemoveListener(Show);
        }

        private void Show()
        {
            flechita.gameObject.SetActive(true);
        }

        private void FixedUpdate()
        {
            if(!ccd.HaTerminado() && flechita.gameObject.activeSelf)
            {
                flechita.gameObject.SetActive(false);
            }
        }
    }
}