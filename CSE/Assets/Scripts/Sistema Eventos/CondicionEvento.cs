using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hexstar.CSE.SistemaEventos
{
    public class CondicionEvento
    {
        public int valor;
        virtual public bool EsValida() { throw new NotImplementedException(); }
        virtual public string ToString(int tabs)
        {
            string aux = "";
            for (int i = 0; i < tabs; i++) aux += "\t";
            return aux + valor.ToString();
        }
    }

    public class CondCaso : CondicionEvento
    {
        public enum TipoResultado { COMPLETADO, PERDIDO, ABANDONADO, INTENTADO}
        public TipoResultado tipo;

        public override bool EsValida()
        {
            int idx = ResourceManager.CasosCompletados.IndexOf(valor);
            if (idx == -1) return false;

            return tipo switch
            {
                TipoResultado.COMPLETADO => ResourceManager.CasosCompletados_ListaDeEstados[idx] ==  1,
                TipoResultado.PERDIDO =>    ResourceManager.CasosCompletados_ListaDeEstados[idx] == -1,
                TipoResultado.ABANDONADO => ResourceManager.CasosCompletados_ListaDeEstados[idx] ==  0,
                TipoResultado.INTENTADO =>  true,
                _ => throw new NotImplementedException(),
            };
        }
        override public string ToString(int tabs)
        {
            string aux = "";
            for (int i = 0; i < tabs; i++) aux += "\t";
            return aux + "CondCaso(" + tipo + "," + valor + ")";
        }
    }

    public class CondDia : CondicionEvento
    {
        public enum Operacion { EQ, LT,GT };
        public Operacion operacion;

        public override bool EsValida()
        {
            return operacion switch { 
                Operacion.EQ => ResourceManager.Dia == valor, 
                Operacion.LT => ResourceManager.Dia < valor, 
                Operacion.GT => ResourceManager.Dia > valor,
                _ => throw new NotImplementedException(),
            };
        }
        override public string ToString(int tabs)
        {
            string aux = "";
            for (int i = 0; i < tabs; i++) aux += "\t";
            return aux + "CondDia(" + operacion + "," + valor + ")";
        }
    }

    public class CondRep : CondDia
    {
        public enum TipoRep { PUEBLO, EMPRESA };
        public TipoRep tipoRep;

        public override bool EsValida()
        {
            if(tipoRep == TipoRep.PUEBLO)
            {
                return operacion switch
                {
                    Operacion.EQ => ResourceManager.ReputacionPueblo == valor,
                    Operacion.LT => ResourceManager.ReputacionPueblo < valor,
                    Operacion.GT => ResourceManager.ReputacionPueblo > valor,
                    _ => throw new NotImplementedException(),
                };
            }
            else //TipoRep.EMPRESA
            {
                return operacion switch
                {
                    Operacion.EQ => ResourceManager.ReputacionEmpresas == valor,
                    Operacion.LT => ResourceManager.ReputacionEmpresas < valor,
                    Operacion.GT => ResourceManager.ReputacionEmpresas > valor,
                    _ => throw new NotImplementedException(),
                };
            }
        }
        override public string ToString(int tabs)
        {
            string aux = "";
            for (int i = 0; i < tabs; i++) aux += "\t";
            return aux + "CondRep(" + tipoRep + "," + valor + ")";
        }
    }

    public class CondDialogE : CondDia
    {
        public string key = "";
        public override bool EsValida()
        {
            string cad = Dialogue.ControladorDialogos.GetDialogueEventValue(key);
            if (cad == "") return false;
            if (int.TryParse(cad, out int contenido))
            {
                return operacion switch
                {
                    Operacion.EQ => contenido == valor,
                    Operacion.LT => contenido < valor,
                    Operacion.GT => contenido > valor,
                    _ => throw new NotImplementedException(),
                };
            }
            else return false;
        }
        override public string ToString(int tabs)
        {
            string aux = "";
            for (int i = 0; i < tabs; i++) aux += "\t";
            return aux + "CondEv(" + key + "," + valor + ")";
        }
    }

    public class CondConect : CondicionEvento
    {
        public enum TipoConect { AND, OR };
        public TipoConect tipoConect;
        public List<CondicionEvento> condiciones = new();

        public override bool EsValida()
        {
            bool valida;
            if (condiciones.Count == 0) return true;
            switch (tipoConect)
            {
                case TipoConect.AND:
                    valida = true;
                    for(int i = 0; i < condiciones.Count; i++)
                        valida &= condiciones[i].EsValida();
                    break;
                case TipoConect.OR:
                    valida = false;
                    for (int i = 0; i < condiciones.Count; i++)
                        valida |= condiciones[i].EsValida();
                    break;
                default:
                    valida = false;
                    break;
            }
            return valida;
        }
        override public string ToString(int tabs)
        {
            string aux = "";
            for (int i = 0; i < tabs; i++) aux += "\t";
            aux += "CondConect(" + tipoConect + ")";
            foreach(CondicionEvento condicion in condiciones)
            {
                aux += "\n" + condicion.ToString(tabs + 1);
            }
            return aux;
        }
    }

    // PARTE DE LA GRAMÁTICA //
    /*
     * CONDICION -> AND | OR | CONDICION_SIMPLE

     * AND -> "AND(" LISTA ")"
     * OR -> "OR(" LISTA ")"

     * LISTA -> CONDICION | CONDICION ',' LISTA

     * CONDICION_SIMPLE -> VAR OP INT
     
     * VAR -> "CASO_COMPLETADO" | "CASO_PERDIDO" | "CASO_ABANDONADO" | "DIA_ACTUAL" | "REP_PUEBLO" | "REP_EMPRESA | (EVENTO_\w+)"
     * OP -> "=" | "<" | ">"
     */
    public static class ParserCondiciones
    {
        static readonly char[] relationalOperations = { '=','<','>' };

        // Nivel 3
        static readonly string num = @"(-?\d+)";
        static readonly string op = @"(=|<|>)";
        static readonly string var = @"(CASO_COMPLETADO|CASO_PERDIDO|CASO_ABANDONADO|CASO_INTENTADO|DIA_ACTUAL|REP_PUEBLO|REP_EMPRESA|(EVENTO_\w+))";
        static readonly string condicion_simple = @"(" +var+op+num+ @")";
        // Nivel 2
        static readonly string and = @"(AND\(.*\))";
        static readonly string or =  @"(OR\(.*\))";
        static readonly string cond = @"(" +or+@"|"+and+@"|"+ condicion_simple + @")";
        // Nivel 1
        static readonly string lista_A = cond;
        static readonly string lista_B = @"("+cond+@",.*)";

        static readonly string s = cond;

        public static string Get_Full_Regex() => s;

        public static CondicionEvento Procesar_a_AST(string condicion)
        {
            condicion = condicion.Replace(" ", "").Replace("\n", "").Replace("\t", "").ToUpper();
            
            Regex r_S = new(s);
            Match m = r_S.Match(condicion);
            if (!m.Success)
            {
                throw new Exception("La condición no sigue la gramática.\nCondición:"+condicion);
            }
            CondicionEvento ast = P_cond(m.Value);
            return ast;
        }

        private static CondicionEvento P_cond(string contenido)
        {
            CondicionEvento nodo = null;

            // ¿Condicion simple?
            Regex r_condicion_simple = new(condicion_simple);
            Match m1 = r_condicion_simple.Match(contenido);
            int pos1 = m1.Index;
            // ¿AND?
            Regex r_and = new(and);
            Match m2 = r_and.Match(contenido);
            int pos2 = m2.Index;
            // ¿OR?
            Regex r_or = new(or);
            Match m3 = r_or.Match(contenido);
            int pos3 = m3.Index;

            if (m1.Success && pos1 <= pos2 && pos1 <= pos3) // Condicion simple
            {
                if (contenido.Length != m1.Length) {
                    throw new Exception("Error.\nEl parser detecta que la Condición_Simple \"" + 
                        m1.Value + "\" no ocupa toda el área del contenido obtenido por el regex.");
                }
                nodo = P_cond_simple(m1.Value);
            }
            else if (m2.Success && pos2 <= pos1 && pos2 <= pos3) // AND
            {
                CondConect cond = new() { tipoConect = CondConect.TipoConect.AND };
                P_lista(contenido[(pos2 + 4)..^1], ref cond.condiciones); // "AND("  "LISTA"  ")"
                nodo = cond;
            }
            else if (m3.Success && pos3 <= pos2 && pos3 <= pos1) // OR
            {
                CondConect cond = new() { tipoConect = CondConect.TipoConect.OR };
                P_lista(contenido[(pos3 + 3)..^1], ref cond.condiciones); //  "OR("  "LISTA"  ")"
                nodo = cond;
            }
            else
            {
                throw new Exception("Error al construir el AST.\n\"" + contenido + 
                    "\" No se detecta como una Condición_Simple, AND o OR.");
            }
            return nodo;
        }

        private static void P_lista(string contenido, ref List<CondicionEvento> listaDelConector)
        {
            Regex r_listaA = new(lista_A);
            Match mA = r_listaA.Match(contenido);
            Regex r_listaB = new(lista_B);
            Match mB = r_listaB.Match(contenido);

            if (mA.Success)
            {
                listaDelConector.Add(P_cond(mA.Value));
            }
            else
            {
                throw new Exception("Error al construir el AST.\n\"" + contenido +
                    "\" Se esperaba una lista de condiciones pero está vacío.");
            }
            if (mB.Success)
            {
                if (mB.Index != 0) return;
                string resto = contenido[(mA.Index + mA.Value.Length + 1)..];
                P_lista(resto, ref listaDelConector);
            }
        }

        private static CondicionEvento P_cond_simple(string contenido)
        {
            string[] pareja = contenido.Split(relationalOperations);
            
            string variable = pareja[0];

            char operador = contenido[variable.Length]; // = / < / >
            CondDia.Operacion op = CondDia.Operacion.EQ;
            if (operador == '<') op = CondDia.Operacion.LT;
            else if (operador == '>') op = CondDia.Operacion.GT;

            int valor = int.Parse(pareja[1], System.Globalization.NumberStyles.Integer);

            if (variable.StartsWith("EVENTO_")) return new CondDialogE() { operacion = op, valor = valor, key = variable[7..] };
            else
            {
                switch (variable)
                {
                    case "CASO_COMPLETADO":
                        if (operador != '=') throw new Exception("Sólo se puede usar el operador '=' con CASO_COMPLETADO, se usó \"" + operador + "\".");
                        return new CondCaso() { tipo = CondCaso.TipoResultado.COMPLETADO, valor = valor };

                    case "CASO_PERDIDO":
                        if (operador != '=') throw new Exception("Sólo se puede usar el operador '=' con CASO_PERDIDO, se usó \"" + operador + "\".");
                        return new CondCaso() { tipo = CondCaso.TipoResultado.PERDIDO, valor = valor };

                    case "CASO_ABANDONADO":
                        if (operador != '=') throw new Exception("Sólo se puede usar el operador '=' con CASO_ABANDONADO, se usó \"" + operador + "\".");
                        return new CondCaso() { tipo = CondCaso.TipoResultado.ABANDONADO, valor = valor };

                    case "CASO_INTENTADO":
                        if (operador != '=') throw new Exception("Sólo se puede usar el operador '=' con CASO_INTENTADO, se usó \"" + operador + "\".");
                        return new CondCaso() { tipo = CondCaso.TipoResultado.INTENTADO, valor = valor };

                    case "DIA_ACTUAL":
                        return new CondDia() { operacion = op, valor = valor };

                    case "REP_PUEBLO":
                        return new CondRep() { operacion = op, valor = valor, tipoRep = CondRep.TipoRep.PUEBLO };

                    case "REP_EMPRESA":
                        return new CondRep() { operacion = op, valor = valor, tipoRep = CondRep.TipoRep.EMPRESA };

                    default:
                        throw new Exception("La variable \"" + variable + "\" no se encuentra en la lista de variables editables.");
                }
            }
        }
    }
}