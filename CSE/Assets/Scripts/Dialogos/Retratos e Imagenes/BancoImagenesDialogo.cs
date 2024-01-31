using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.Dialogue
{
    [CreateAssetMenu(fileName = "BID", menuName ="Hexstar/Dialogue/Banco de imagenes")]
    public class BancoImagenesDialogo : ScriptableObject
    {
        [System.Serializable]
        public struct ImagenD
        {
            public string tag;
            public Sprite sprite;
        }

        [SerializeField] List<ImagenD> imagenes = new();

        public Sprite GetSprite(string tag)
        {
            return imagenes.Find((img) => { return img.tag == tag; }).sprite;
        }
    }
}