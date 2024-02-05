using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.CSE.Informes
{
    public class CarpetaInformesController : MonoBehaviour
    {
        internal static CarpetaInformesController Instance { get; private set; }

        [SerializeField] TextMeshProUGUI tituloCaso;
        [SerializeField] TextMeshProUGUI descripcionCaso;
        [SerializeField] TextMeshProUGUI listaRecompensas;
        [SerializeField] TextMeshProUGUI pistasCaso;
        [SerializeField] TextMeshProUGUI contadorPagPistas;
        [SerializeField] Button boton_pagSig;
        [SerializeField] Button boton_pagAnt;
        [SerializeField] Button boton_pagPistaSig;
        [SerializeField] Button boton_pagPistaAnt;

        [SerializeField] Boton3D seleccionCarpeta;

        Animator anim;
        readonly string cambioDePagina  = "cambioDePagina";
        readonly string mostrarEnCajon  = "mostrarEnCajon";
        readonly string esconderEnCajon = "esconderEnCajon";
        readonly string sacarDeCajon    = "sacarDeCajon";
        readonly string devolverACajon  = "devolverACajon";
        private bool carpetaEnCajon = true;

        public static List<Informe> Informes { get; private set; } = new();
        static int paginaActual = -1;

        public void MostrarEnCajon()
        {
            if (carpetaEnCajon) anim.Play(mostrarEnCajon);
            else anim.Play(devolverACajon);
            carpetaEnCajon = true;
        }
        public void EsconderEnCajon()
        {
            if (carpetaEnCajon) anim.Play(esconderEnCajon);
        }
        public void SacarDeCajon()
        {
            carpetaEnCajon = false;
            FijarEstadoCamara();
            anim.Play(sacarDeCajon);
        }

        public static void ForzarPaginaInforme(int idx)
        {
            if(idx < 0 || idx >= Informes.Count)
            {
                Debug.LogError("Se ha intentado acceder a un informe que no existe");
                return;
            }
            paginaActual = idx;
            Instance.MostrarInforme();
        }
        public static int IndiceDeInformeCorrespondiente(Caso c)
        {
            return Informes.FindIndex((ctx) => { return ctx.id == c.id; });
        }

        private void PistaPaginaSiguiente()
        {
            pistasCaso.pageToDisplay++;
            if (pistasCaso.pageToDisplay >= pistasCaso.textInfo.pageCount) pistasCaso.pageToDisplay = pistasCaso.textInfo.pageCount;
            ActualizarContadorPaginasPistas();
        }
        private void PistaPaginaAnterior()
        {
            pistasCaso.pageToDisplay--;
            if (pistasCaso.pageToDisplay < 1) pistasCaso.pageToDisplay = 1;
            ActualizarContadorPaginasPistas();
        }

        private void InformePaginaSiguiente()
        {
            paginaActual++;
            if (paginaActual >= Informes.Count) paginaActual = Informes.Count - 1;
            anim.Play(cambioDePagina);
        }
        private void InformePaginaAnterior()
        {
            paginaActual--;
            if (paginaActual < 0) paginaActual = 0;
            anim.Play(cambioDePagina);
        }

        public void PrepararCarpeta()
        {
            Caso c = PuzzleManager.GetCasoActivo();
            if (c != null) paginaActual = IndiceDeInformeCorrespondiente(c);
            MostrarInforme();
        }
        public void MostrarInforme()
        {
            if (paginaActual >= 0 && paginaActual < Informes.Count)
            {
                var inf = Informes[paginaActual];
                tituloCaso.SetText(inf.FormarTitulo());
                descripcionCaso.SetText(inf.descripcion);
                listaRecompensas.SetText(inf.FormarListaRecompensas());
                pistasCaso.SetText(inf.FormarPistasCaso());
                pistasCaso.pageToDisplay = 1;
                ActualizarContadorPaginasPistas();
            }
            else
            {
                tituloCaso.SetText("");
                descripcionCaso.SetText("");
                listaRecompensas.SetText("");
                pistasCaso.SetText("");
                contadorPagPistas.SetText("");
            }
            ActualizarBotones();
        }
        private void ActualizarBotones()
        {
            boton_pagAnt.interactable = Informes.Count > 0 && paginaActual > 0;
            boton_pagSig.interactable = Informes.Count > 0 && paginaActual < Informes.Count - 1;

            int pag = pistasCaso.pageToDisplay;
            boton_pagPistaAnt.interactable = Informes.Count > 0 && pag > 1;
            boton_pagPistaSig.interactable = Informes.Count > 0 && pag < pistasCaso.textInfo.pageCount;
        }
        private void ActualizarContadorPaginasPistas()
        {
            int n = pistasCaso.textInfo.pageCount > 0 ? pistasCaso.textInfo.pageCount : 1;
            contadorPagPistas.SetText(pistasCaso.pageToDisplay + "/" + n);
        }

        public void FijarEstadoCamara()
        {
            InscryptionLikeCameraState.Instance.SetEstadoActual(0);
            InscryptionLikeCameraState.Instance.GetCamState().Transition(0);
            InscryptionLikeCameraState.SetBypass(true);
        }
        public void LiberarEstadoCamara()
        {
            InscryptionLikeCameraState.SetBypass(false);
        }

        private void Awake()
        {
            Instance = this;

            anim = GetComponent<Animator>();
            
            boton_pagSig.onClick.AddListener(InformePaginaSiguiente);
            boton_pagAnt.onClick.AddListener(InformePaginaAnterior);

            boton_pagPistaSig.onClick.AddListener(PistaPaginaSiguiente);
            boton_pagPistaAnt.onClick.AddListener(PistaPaginaAnterior);

            seleccionCarpeta.onClick.AddListener(SacarDeCajon);
        }
    }

    [System.Serializable]
    public struct Informe
    {
        internal int id;
        internal string titulo;
        internal string descripcion;
        internal Bounty[] recompensas;
        internal bool[] recompensasCompletadas;
        internal string[] descripcionesPistas;

        public Informe(Caso c)
        {
            id = c.id;
            titulo = c.titulo;
            descripcion = c.resumen;
            recompensas = (Bounty[])c.bounties.Clone();
            recompensasCompletadas = new bool[recompensas.Length];
            descripcionesPistas = new string[c.pistas.Length];
            for (int i = 0; i < c.pistas.Length; i++) descripcionesPistas[i] = c.pistas[i].descripcion;
        }
        public void SetBountyCompletion(int idx, bool completed)
        {
            if (idx < 0 || idx >= recompensasCompletadas.Length)
            {
                Debug.LogError("Se ha intentado indicar una recompensa en la carpeta que está fuera de rango...");
                return;
            }
            recompensasCompletadas[idx] = completed;
        }
        internal string FormarTitulo()
        {
            return id + ". " + titulo;
        }
        internal string FormarListaRecompensas()
        {
            StringBuilder sb = new();
            for(int i=0; i < recompensas.Length; i++)
            {
                sb.Append(recompensasCompletadas[i] ? "<#44FF44>": "<#CC0000>");
                recompensas[i].AddToStringBuilder(ref sb);
                sb.Append("<#000000>");
            }
            return sb.ToString();
        }
        internal string FormarPistasCaso()
        {
            StringBuilder sb = new();
            for(int i =0; i < descripcionesPistas.Length; i++)
            {
                sb.Append("<b>");
                sb.Append(i);
                sb.Append(".</b> ");
                sb.Append(descripcionesPistas[i]);
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}