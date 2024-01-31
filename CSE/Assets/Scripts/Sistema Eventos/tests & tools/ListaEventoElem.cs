using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.UI
{
    public class ListaEventoElem : MonoBehaviour
    {
        public Button boton;
        public int pointing_to = 0;
        public static UnitTest_Eventos ute;

        public void Setup()
        {
            boton.onClick.AddListener(() => ute.SelectEvent(pointing_to));
        }

        private void Awake()
        {
            boton = GetComponent<Button>();
        }
    }
}