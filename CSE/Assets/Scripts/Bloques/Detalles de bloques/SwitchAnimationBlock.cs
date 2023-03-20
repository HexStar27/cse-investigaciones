using UnityEngine;

namespace Hexstar.CSE {
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