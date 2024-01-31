using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.Dialogue {
    [System.Serializable,
     CreateAssetMenu(fileName = "Dialogo",menuName = "Hexstar/Dialogue/Dialogo")]
    public class Dialogo : ScriptableObject
    {

        public DialogueDataBase ddb;
        [SerializeField] List<ActorDialogo> actores = new();
        public List<int> dialogo = new();
        [SerializeField, Multiline] private string sample;
        [Header("Opcional")]
        public int idxInicial;
        public int distancia;

        private void OnValidate()
        {
            RefreshLooks();
        }

        public void RefreshLooks()
        {
            sample = "";
            for (int i = 0; i < dialogo.Count; i++)
            {
                int idx = dialogo[i];
                string val;
                if (idx >= 0 && idx < ddb.entries.Count) val = ddb.entries[idx].txt;
                else val = "Index out of DataBase";
                sample += val+"\n";
            }
        }

        public void RellenarSegunRango(bool limpiar)
        {
            if (ddb == null) return;
            int n = ddb.entries.Count;
            if (idxInicial < 0 || idxInicial >= n) return;
            if(limpiar) dialogo.Clear();
            int limite = Math.Min(idxInicial+distancia,n);
            for (int i = idxInicial; i < limite; i++)
            {
                dialogo.Add(i);
            }
        }

        public Sprite GetPortrait(string actor, string expression)
        {
            var a = actores.Find((i) => { return i.name == actor; });
            if (a != null) return a.GetExpression(expression);
            return null;
        }
    }
}