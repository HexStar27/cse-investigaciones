using UnityEngine;
using TMPro;
using System.Text;
using System;

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
        if(_retos.spriteAsset == null)
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

    public static string Cond2Str(Caso.Bounty.Condicion cond)
    {
        return cond switch
        {
            Caso.Bounty.Condicion.Ganar => "Gana el caso",
            Caso.Bounty.Condicion.Perder => "PIERDE A POSTA",
            Caso.Bounty.Condicion.MaxConsultasT1 => "Usa 4 consultas o menos",
            Caso.Bounty.Condicion.MaxConsultasT2 => "Usa 2 consultas o menos",
            Caso.Bounty.Condicion.TiempoLimiteT1 => "Termina en 3m 30s o menos",
            Caso.Bounty.Condicion.TiempoLimiteT2 => "Termina en 2m 30s o menos",
            Caso.Bounty.Condicion.TiempoLimiteT3 => "Termina en 1m 30s o menos",
            Caso.Bounty.Condicion.TiempoLimiteT4 => "Termina en 45s o menos",
            Caso.Bounty.Condicion.Siempre => "Garantizado",
            _ => "ERROR"
        };
    }
    public static string Efecto2Str(Caso.Bounty.TipoEfecto efecto)
    {
        return efecto switch
        {
            Caso.Bounty.TipoEfecto.AGENTES_DISPONIBLES => "<sprite name=\"icono_mas_agentes\">",
            Caso.Bounty.TipoEfecto.CONSULTAS_DISPONIBLES => "<sprite name=\"icono_mas_consultas\">",
            Caso.Bounty.TipoEfecto.CONSULTAS_MAXIMAS => "<sprite name=\"icono_mas_max_consultas\">",
            Caso.Bounty.TipoEfecto.REPUTACION_PUEBLO => "<sprite name=\"icono_reputacion_pueblo\">",
            Caso.Bounty.TipoEfecto.RESPUTACION_EMPRESA => "<sprite name=\"icono_reputacion_empresa\">",
            _ => "ERROR",
        };
    }

    private string Retos2String(Caso c)
    {
        StringBuilder sb = new();
        if(c.bounties == null) throw new NullReferenceException();

        foreach(var item in c.bounties)
        {
            sb.Append(" > ");
            sb.Append(Cond2Str(item.condicion));
            sb.Append(": ");
            sb.Append(Efecto2Str(item.efecto));
            sb.Append($"{item.cantidad:+#;-#;+0}\n");
        }

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
