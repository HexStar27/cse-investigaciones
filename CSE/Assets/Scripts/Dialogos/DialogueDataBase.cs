using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.Dialogue
{
    [System.Serializable,
     CreateAssetMenu(fileName = "DDB",menuName = "Hexstar/Dialogue/DataBase")]
    public class DialogueDataBase : ScriptableObject
    {
        public TextAsset asset;
        public List<Entry> entries = new();
        [SerializeField] private List<string> labels = new();
        [SerializeField] private List<int> labelIndex = new();

        public int GetFirstIndexOfGroup(string group)
        {
            return entries.FindIndex((Entry e) => { return e.group.Equals(group); });
        }
        public int GetIndexOfLabel(string label)
        {
            int idx = labels.IndexOf(label);
            if (idx >= 0)
                return labelIndex[idx];
            else
                return -1;
        }
        /// <summary>
        /// Busca la etiqueta y devuelve su índice.
        /// Tienen prioridades las etiquetas del mismo grupo que el de la entrada ejecutada.
        /// Devuelve -1 si no encuentra nada.
        /// </summary>
        public int GetIndexOfLabelByGroup(int currentEntryIdx, string label)
        {
            if (currentEntryIdx < 0 || currentEntryIdx >= entries.Count) return -1;
            string gt = entries[currentEntryIdx].group;

            int alternativeIdx = -1;
            for(int i = 0; i < labels.Count; i++)
            {
                if (labels[i] == label)
                {
                    string gl = entries[labelIndex[i]].group;
                    if (gl.Equals(gt)) return labelIndex[i];
                    else alternativeIdx = labelIndex[i];
                }
            }
            return alternativeIdx;
        }

        public void LoadFromJSON(string ddbContent)
        {
            var json = JSON.Parse(ddbContent);
            if (json == null) throw new Exception("No se ha podido parsear el DDB correctamente como un JSON.");
            
            entries.Clear();
            labels.Clear();
            labelIndex.Clear();

            int n = json.Count;
            for (int i = 0; i < n; i++)
            {
                var jsonEntry = json[i];
                string d, fx = "", g = "", et = "";
                if (!jsonEntry.HasKey("Dialogo")) throw new Exception("La clave \"Dialogo\" es obligatoria en cada entrada de la DDB.");
                
                d = jsonEntry["Dialogo"].Value;
                if (jsonEntry.HasKey("Efectos")) fx = jsonEntry["Efectos"].Value;
                if (jsonEntry.HasKey("Grupo")) g = jsonEntry["Grupo"].Value;
                if (jsonEntry.HasKey("Etiqueta")) et = jsonEntry["Etiqueta"].Value;

                if (!et.Equals(""))
                {
                    labels.Add(et);
                    labelIndex.Add(entries.Count);
                }
                entries.Add(new Entry(d, LoadFXs(fx), g));
            }
        }
        public void LoadFromString(string ddbContent)
        {
            LoadDDB(CSVReader.ReadString(ddbContent));
        }
        public void LoadFromAsset()
        {
            if (asset == null) return;
            LoadDDB(CSVReader.Read(asset));
        }
        private void LoadDDB(List<Dictionary<string, object>> data)
        {
            int n = data.Count;
            if (n <= 0)
            {
                Debug.LogError("Data not found trying to load a dialogue database");
                return;
            }

            entries.Clear();
            labels.Clear();
            labelIndex.Clear();

            for (int i = 0; i < n; i++)
            {
                var row = data[i];

                var entrada = new Entry((string)row["Dialogo"], LoadFXs((string)row["Efectos"]), (string)row["Grupo"]);
                string etiqueta = (string)row["Etiqueta"];

                if (!etiqueta.Equals(""))
                {
                    labels.Add(etiqueta);
                    labelIndex.Add(entries.Count);
                }
                entries.Add(entrada);
            }
        }

        public static List<DialogueFX> LoadFXs(string cadena)
        {
            List<DialogueFX> fxs = new();
            if (cadena == "") return fxs;
            var efectos = SplitFXs(cadena);
            for (int i = 0; i < efectos.Length; i++)
            {
                string efecto = efectos[i].Trim(' ',',');
                int eq = efecto.IndexOf('=');
                if (eq < 0) throw new Exception("El efecto número " + i + " no tiene una asignación '='");
                string tipo = efecto[0..eq];
                string parametros = efecto[(eq + 1)..];

                DialogueFX fx = new();
                fx.tipo = tipo.Trim().ToUpper() switch
                {
                    "CAMBIO PERFIL" => DialogueFX.Tipo.CambioPerfil,
                    "MOSTRAR IMAGEN" => DialogueFX.Tipo.MostrarImagen,
                    "RAMIFICAR" => DialogueFX.Tipo.Ramificar,
                    "DAR OPCIONES" => DialogueFX.Tipo.DarOpciones,
                    "OSCURECER" => DialogueFX.Tipo.Oscurecer,
                    "IR A ENTRADA" => DialogueFX.Tipo.IrAEntrada,
                    "OTRO EFECTO" => DialogueFX.Tipo.OtroEfecto,
                    "ESTABLECER EVENTO" => DialogueFX.Tipo.EstablecerEvento,
                    _ => DialogueFX.Tipo.NULL_EV,
                };
                if (fx.tipo == DialogueFX.Tipo.NULL_EV) throw new Exception("\""+tipo.Trim()+"\" NO es un efecto válido");
                fx.value = parametros.Trim(' ', '\"', '(', ')');
                fxs.Add(fx);
            }
            return fxs;
        }

        private static string[] SplitFXs(string line)
        {
            List<string> results = new();

            int caret = 0;
            bool insideOfString = false;
            int n = line.Length;
            for(int i = 0; i < n; i++)
            {
                if (line[i] == '\"' || line[i] == '(' || line[i] == ')') insideOfString = !insideOfString;
                else if(!insideOfString && line[i] == ',')
                {
                    results.Add(line[caret..i]);
                    caret = i;
                }
            }
            results.Add(line[caret..]);
            if (insideOfString) throw new Exception("Se abrió una '\"' pero no se ha cerrado");

            return results.ToArray();
        }

        [System.Serializable] public class Entry
        {
            public string txt;
            public List<DialogueFX> fx;
            public string group;
            public Entry(string texto, List<DialogueFX> efectos, string grupo = "")
            {
                txt = texto;
                fx = efectos;
                group = grupo;
            }
            public bool InGroup(string otherGroup) => group.Equals(otherGroup);
        }
    }

    [System.Serializable]
    public struct DialogueFX
    {
        public enum Tipo { CambioPerfil, MostrarImagen, Ramificar, DarOpciones, IrAEntrada, Oscurecer, OtroEfecto, EstablecerEvento, NULL_EV };
        public Tipo tipo;
        public string value;
    }
}