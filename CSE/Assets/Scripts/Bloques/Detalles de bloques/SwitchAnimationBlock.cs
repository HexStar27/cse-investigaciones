using UnityEngine;

namespace Hexstar.CSE {
    /// <summary>
    /// Ejecuta la animación de abrir o cerrar el socket de consulta del escritorio.
    /// </summary>
    public class SwitchAnimationBlock : MonoBehaviour
    {
        [SerializeField] BlockMovAndConexion controller;
        [SerializeField] Animator anim;
        [SerializeField] string openClip, closeClip;
        bool cerrado = false;

        private void Switch()
        {
            cerrado = !cerrado;
            if(cerrado) anim.Play(closeClip);
            else anim.Play(openClip);
        }

        private void OnEnable()
        {
            controller.onBlockConectionChanged.AddListener(Switch);
        }
        private void OnDisable()
        {
            controller.onBlockConectionChanged.RemoveListener(Switch);
        }
    }
}