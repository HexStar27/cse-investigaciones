using UnityEngine;

namespace Hexstar.CSE
{
    [CreateAssetMenu(fileName = "AlmacenBloques", menuName = "Hexstar/Bloques/AlmacenBloques")]
    public class AlmacenDeBloques : ScriptableObject
    {
        public DatosBloque[] datosBloques = new DatosBloque[0];

        public int[] dificultyToBlocksAvailable = new int[0];
    }
}