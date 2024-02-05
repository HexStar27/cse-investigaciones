using UnityEngine;
using TMPro;
using System.Text;
using System;

namespace Hexstar.CSE
{
    public class CasoDescripcion : MonoBehaviour
    {
        readonly string AgenteSpriteText = "<sprite name=\"icono_agentes\">";
        readonly string AgenteSpriteTextRojo = "<sprite name=\"icono_agentes\" color=#FF0000>";
        public static CasoDescripcion Instance { get; private set; }
        [SerializeField] GameObject _panel;
        [SerializeField] TextMeshProUGUI _titulo;
        [SerializeField] TextMeshProUGUI _resumen;
        [SerializeField] TextMeshProUGUI _retos;
        [SerializeField] TextMeshProUGUI _coste;
        public HighScoreTable panelPuntuaciones;
        CasoMapa _casoMapaActual;

        private void Awake()
        {
            Instance = this;
            if (_retos.spriteAsset == null)
                _retos.spriteAsset = Resources.Load("Resources/Sprite Assets/GVars_CSE_Icons") as TMP_SpriteAsset;
        }

        public void LeerCaso(CasoMapa cm, Caso c, int idx)
        {
            _casoMapaActual = cm;
            _titulo.SetText(c.titulo);
            _resumen.SetText(c.resumen);
            _retos.SetText(Retos2String(c));
            _coste.SetText(Coste2String(c.coste));

            int from = 0;
            if (idx > 0) from = PuzzleManager.PuntuacionesPorCaso[idx - 1];
            panelPuntuaciones.ShowOnlyRange(from, PuzzleManager.PuntuacionesPorCaso[idx]);
        }

        public void Abrir(bool value)
        {
            _panel.SetActive(value);
        }
        public void ComprarCasoMostrado() => _casoMapaActual.Comprar();

        private string Retos2String(Caso c)
        {
            StringBuilder sb = new();
            if (c.bounties == null) throw new NullReferenceException();

            foreach (var item in c.bounties) item.AddToStringBuilder(ref sb);

            if (c.bounties.Length == 0) sb.AppendLine("Sin recompensas...");

            return sb.ToString();
        }
        private string Coste2String(int coste)
        {
            string txt = "";
            for (int i = 0; i < coste; i++)
            {
                txt += i < ResourceManager.AgentesDisponibles ? AgenteSpriteText : AgenteSpriteTextRojo;
            }
            return txt;
        }
    }
}