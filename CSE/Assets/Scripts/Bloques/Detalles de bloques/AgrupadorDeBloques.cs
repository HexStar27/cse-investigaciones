using UnityEngine;

namespace Hexstar.CSE {
    public class AgrupadorDeBloques : MonoBehaviour
    {
        [SerializeField] GameObject bloqueConsultaPrefab;
        [SerializeField] string selectTag = "SELECT";
        BlockMovAndConexion bloque;

        [SerializeField] LineRenderer[] lines = new LineRenderer[4];
        private readonly Vector3 defaultPos = new Vector3(0f, 0.02f, 0f);
        bool raysDisabled = true;

        public void ManipulateBlock()
        {
            if (bloque == null) return;

            Transform grupo = bloque.CQU.GetGroupBlockAssignedTo();
            if (grupo != null) // Desagrupar
            {
                grupo.gameObject.SetActive(true);
                grupo.SetParent(null,true);
                Destroy(bloque.gameObject);
            }
            else // Agrupar
            {
                string title = bloque.CQU.GetPartialQuery();
                GameObject gb = Instantiate(bloqueConsultaPrefab, transform.position, Quaternion.identity);
                var queryData = gb.GetComponent<ContentQueryUnit>();
                queryData.GetContentFromIndex(0).text = title;
                queryData.AssignGroup(bloque.transform);
                bloque.transform.SetParent(gb.transform, true);
                bloque.gameObject.SetActive(false);
            }
        }

        private void UpdateRays()
        {
            if(bloque == null)
            {
                foreach( var line in lines) line.gameObject.SetActive(false);
                raysDisabled = true;
            }
            else
            {
                if(raysDisabled)
                {
                    foreach (var line in lines) line.gameObject.SetActive(true);
                    raysDisabled = false;
                }
                Vector3 pos = bloque.transform.position - transform.position;
                pos.y += 0.08f;
                foreach( var line in lines)
                {
                    line.SetPosition(1,pos);
                }
            }
        }

        private void OnCollisionEnter(Collision coll)
        {
            if (coll.collider.CompareTag(selectTag))
            {
                bloque = coll.collider.GetComponent<BlockMovAndConexion>();
            }
        }

        private void OnCollisionStay(Collision coll)
        {
            UpdateRays();
        }

        private void OnCollisionExit(Collision coll)
        {
            if (coll.collider.CompareTag(selectTag))
            {
                if (bloque != null)
                {
                    if (coll.gameObject == bloque.gameObject) bloque = null;
                    UpdateRays();
                }
            }
        }
    }
}