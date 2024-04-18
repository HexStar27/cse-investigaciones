using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.UI
{
    public class DE_AddButton : MonoBehaviour
    {
        public Button b;

        private void Awake()
        {
            b.onClick.AddListener(()=>{
                DialogueEditorHelper.AddNewEmptyRow();
                transform.parent.position += new Vector3(0, 70);
            });
        }
    }
}