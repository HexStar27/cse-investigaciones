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

        private List<DialogueFX> LoadFXs(string cadena)
        {
            List<DialogueFX> fxs = new();
            var efectos = SplitFXs(cadena);

            for (int i = 0; i < efectos.Length; i++)
            {
                string efecto = efectos[i].Trim(' ',',');
                int eq = efecto.IndexOf('=');
                if (eq < 0) continue;
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
                    _ => DialogueFX.Tipo.OtroEfecto,
                };
                fx.value = parametros.Trim(' ', '\"', '“', '”');
                fxs.Add(fx);
            }
            return fxs;
        }

        private string[] SplitFXs(string line)
        {
            List<string> results = new();

            int caret = 0;
            bool insideOfString = false;
            int n = line.Length;
            for(int i = 0; i < n; i++)
            {
                if (line[i] == '\"' || line[i] == '“' || line[i] == '”') insideOfString = !insideOfString;
                else if(!insideOfString && line[i] == ',')
                {
                    results.Add(line[caret..i]);
                    caret = i;
                }
            }
            results.Add(line[caret..]);

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
        public enum Tipo { CambioPerfil, MostrarImagen, Ramificar, DarOpciones, IrAEntrada, Oscurecer, OtroEfecto, EstablecerEvento };
        public Tipo tipo;
        public string value;
    }
}