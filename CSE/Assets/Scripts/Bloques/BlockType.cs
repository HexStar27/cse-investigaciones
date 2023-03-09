using UnityEngine;

namespace Hexstar.CSE
{
	[CreateAssetMenu(fileName = "BlockType", menuName = "Block/BlockType")]
	public class BlockType : ScriptableObject
	{
		public BlockType superior;
		public bool esSeccion;

		public bool PerteneceA(BlockType otro)
		{
			BlockType aux = this;
			for (int iter = 0; iter < 10 && aux != null; iter++)
			{
				if (aux.name == otro.name) return true;
				aux = aux.superior;
			}
			return false;
		}
	}
}