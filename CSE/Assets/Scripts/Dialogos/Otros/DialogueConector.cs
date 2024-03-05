using UnityEngine;

namespace Hexstar.Dialogue
{
    public class DialogueConector : MonoBehaviour
    {
        [SerializeField] ControladorDialogos controladorDialogos = null;
        [SerializeField] ElementoSeparador separador = null;
        
        private void Terminar() { separador.Terminar(); }

        private void OnEnable()
        {
            controladorDialogos.onDialogueFinish.AddListener(Terminar);
        }
        private void OnDisable()
        {
            controladorDialogos.onDialogueFinish.RemoveListener(Terminar);
        }
    }
}