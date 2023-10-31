using UnityEngine;

namespace Hexstar.CSE
{
    public class SpecialCaseNOT : MonoBehaviour
    {
        [SerializeField] BlockData bdOP;
        [SerializeField] BlockData bdF;
        BlockMovAndConexion bmc;
        BotonConfigOperadores bco;

        private void Awake()
        {
            bmc = GetComponent<BlockMovAndConexion>();
            bco = GetComponent<BotonConfigOperadores>();
        }

        void EstablecerNOT(bool val)
        {
            if(val) bmc.SetBlockData(bdF);
            else bmc.SetBlockData(bdOP);
            bmc.UpdateVisuals();
        }

        private void CalcularBlockData(string content)
        {
            EstablecerNOT(content == "NOT");
        }

        private void OnEnable()
        {
            bco.onSelectOP.AddListener(CalcularBlockData);
        }
    }
}