using System.Text;
using UnityEngine;

namespace Hexstar.CSE
{
    public class ContentQueryUnit : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshPro[] content;
        BlockMovAndConexion controlador;
        Transform queryGroupItRepresents = null;

        public void Awake()
        {
            controlador = GetComponent<BlockMovAndConexion>();
        }

        public TMPro.TextMeshPro GetContentFromIndex(int idx)
        {
            if (idx >= 0 && idx < content.Length) return content[idx];
            else return null;
        }

        public void AssignGroup(Transform block)
        {
            queryGroupItRepresents = block;
        }
        public Transform GetGroupBlockAssignedTo()
        {
            return queryGroupItRepresents;
        }

        public string GetPartialQuery()
        {
            StringBuilder pq = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
                pq.Append(content[i].text);
            
            Conector sig = controlador.GetConector(Conector.ConexionType.RIGHT);
            if (sig.EstaConectado())
                pq.Append(" " + sig.ConectorConectado().ElBloque().CQU.GetPartialQuery());
            
            sig = controlador.GetConector(Conector.ConexionType.BOTTOM);
            if(sig.EstaConectado())
                pq.Append("\n" + sig.ConectorConectado().ElBloque().CQU.GetPartialQuery());
            
            return pq.ToString();
        }

        public string GetBlockTextContent(int idx = 0)
        {
            if (idx < 0 || idx >= content.Length) return null;
            return content[idx].text;
        }
    }
}