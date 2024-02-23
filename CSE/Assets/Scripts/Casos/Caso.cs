using Hexstar.CSE;
using Hexstar.CSE.Informes;
using System.Text;
using System.Threading.Tasks;

namespace Hexstar.CSE
{
    [System.Serializable]
    public class Caso
    {
        public int id, dif, coste;
        public bool secundario;
        public bool examen;
        public string titulo;
        public string resumen;

        public DatosPista[] pistas;
        public Bounty[] bounties;

        /// <summary>
        /// Comprueba que se cumplan las condiciones de todos los bounties y los ejecuta.
        /// </summary>
        public async Task ComprobarYAplicarBounties(bool ganado, int consultasUsadas, float tiempoEmpleado)
        {
            if (bounties == null) return;
            int idxInforme = CarpetaInformesController.IndiceDeInformeCorrespondiente(this);
            for (int i = 0; i < bounties.Length; i++)
            {
                bool superado = await bounties[i].ComprobarYAplicar(ganado, consultasUsadas, tiempoEmpleado);
                CarpetaInformesController.Informes[idxInforme].SetBountyCompletion(i, superado);
            }
        }
    }


    [System.Serializable]
    public struct DatosPista
    {
        public string titulo; public string descripcion; public string palabra;
    }


    [System.Serializable]
    public struct Bounty
    {
        public enum TipoEfecto
        {
            AGENTES_DISPONIBLES = 0, CONSULTAS_DISPONIBLES, CONSULTAS_MAXIMAS,
            REPUTACION_PUEBLO, REPUTACION_EMPRESA
        };
        public enum Condicion
        {
            Ganar = 0, Perder = 1, MaxConsultasT1 = 2, MaxConsultasT2 = 3, TiempoLimiteT1 = 4,
            TiempoLimiteT2 = 5, TiempoLimiteT3 = 6, TiempoLimiteT4 = 7, Siempre = 8
        };

        public Condicion condicion;
        public TipoEfecto efecto;
        public int cantidad;

        /// <summary>
        /// Comprueba si se cumple la condición y aplica el efecto si se da el caso.
        /// </summary>
        /// <param name="ganado">Se ha resuelto el caso</param>
        /// <param name="consultasUsadas">Número de consultas usadas para terminar el caso</param>
        /// <param name="tiempoEmpleado">Tiempo que ha tardado el jugador en terminar el caso (en segundos)</param>
        internal async Task<bool> ComprobarYAplicar(bool ganado, int consultasUsadas, float tiempoEmpleado)
        {
            bool superado = condicion switch
            {
                Condicion.Ganar => ganado,
                Condicion.Perder => !ganado,
                Condicion.MaxConsultasT1 => consultasUsadas <= 4,
                Condicion.MaxConsultasT2 => consultasUsadas <= 2,
                Condicion.TiempoLimiteT1 => tiempoEmpleado <= 210f, // 3m 30s
                Condicion.TiempoLimiteT2 => tiempoEmpleado <= 150f, // 2m 30s
                Condicion.TiempoLimiteT3 => tiempoEmpleado <= 90f,  // 1m 30s
                Condicion.TiempoLimiteT4 => tiempoEmpleado <= 45f,  // 45s
                Condicion.Siempre => true,
                _ => true,
            };

            await BountyMessageController.SendMessage(this, superado);

            if (superado)
            {
                switch (efecto)
                {
                    case TipoEfecto.AGENTES_DISPONIBLES:
                        ResourceManager.AgentesDisponibles += cantidad;
                        await DataUpdater.Instance.ShowAgentesDisponibles();
                        break;
                    case TipoEfecto.CONSULTAS_DISPONIBLES:
                        ResourceManager.ConsultasDisponibles += cantidad;
                        await DataUpdater.Instance.ShowConsultasDisponibles();
                        break;
                    case TipoEfecto.CONSULTAS_MAXIMAS:
                        ResourceManager.ConsultasMaximas += cantidad;
                        await DataUpdater.Instance.ShowConsultasMaximas();
                        break;
                    case TipoEfecto.REPUTACION_PUEBLO:
                        ResourceManager.ReputacionPueblo += cantidad;
                        break;
                    case TipoEfecto.REPUTACION_EMPRESA:
                        ResourceManager.ReputacionEmpresas += cantidad;
                        break;
                }
                PuzzleManager.NRetosCumplidos++;
            }
            return superado;
        }
        public bool SoloComprobar(bool ganado, int consultasUsadas, float tiempoEmpleado)
        {
            return condicion switch
            {
                Condicion.Ganar => ganado,
                Condicion.Perder => !ganado,
                Condicion.MaxConsultasT1 => consultasUsadas <= 4,
                Condicion.MaxConsultasT2 => consultasUsadas <= 2,
                Condicion.TiempoLimiteT1 => tiempoEmpleado <= 210f, // 3m 30s
                Condicion.TiempoLimiteT2 => tiempoEmpleado <= 150f, // 2m 30s
                Condicion.TiempoLimiteT3 => tiempoEmpleado <= 90f,  // 1m 30s
                Condicion.TiempoLimiteT4 => tiempoEmpleado <= 45f,  // 45s
                Condicion.Siempre => true,
                _ => true,
            };
        }

        public void AddToStringBuilder(ref StringBuilder sb)
        {
            sb.Append("~ ");
            sb.Append(Cond2Str(condicion));
            sb.Append($"({cantidad:+#;-#;+0}");
            sb.Append(Efecto2Str(efecto));
            sb.Append(")\n");
        }

        public static string Cond2Str(Condicion cond)
        {
            return cond switch
            {
                Condicion.Siempre => "Garantizado    ",
                Condicion.Ganar => "Gana el caso   ",
                Condicion.Perder => "PIERDE A POSTA ",
                Condicion.MaxConsultasT1 => "<sprite name=\"icono_consultas\"> <= 4      ",
                Condicion.MaxConsultasT2 => "<sprite name=\"icono_consultas\"> <= 2      ",
                Condicion.TiempoLimiteT1 => "<sprite name=\"icono_cronometro\"> <= 3:30  ",
                Condicion.TiempoLimiteT2 => "<sprite name=\"icono_cronometro\"> <= 2:30  ",
                Condicion.TiempoLimiteT3 => "<sprite name=\"icono_cronometro\"> <= 1:30  ",
                Condicion.TiempoLimiteT4 => "<sprite name=\"icono_cronometro\"> <= 0:45  ",
                _ => "-#.ERROR.#-"
            };
        }
        public static string Efecto2Str(TipoEfecto efecto)
        {
            return efecto switch
            {
                TipoEfecto.AGENTES_DISPONIBLES => "<sprite name=\"icono_mas_agentes\">",
                TipoEfecto.CONSULTAS_DISPONIBLES => "<sprite name=\"icono_mas_consultas\">",
                TipoEfecto.CONSULTAS_MAXIMAS => "<sprite name=\"icono_mas_max_consultas\">",
                TipoEfecto.REPUTACION_PUEBLO => "<sprite name=\"icono_reputacion_pueblo\">",
                TipoEfecto.REPUTACION_EMPRESA => "<sprite name=\"icono_reputacion_empresa\">",
                _ => "ERROR",
            };
        }
    };
}