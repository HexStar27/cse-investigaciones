using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.CSE
{

	[CreateAssetMenu(fileName = "BlockData", menuName = "Block/BlockData")]
	public class BlockData : ScriptableObject
	{
		public string title;
		[Header("Habilitado de Conectores")]
		public bool TOP;
		public bool LEFT,BOTTOM,RIGHT;
		[Header("Relacionado con tipos")]
		public BlockType tipoPropio;
		[Serializable]
		public struct TipoAceptadoPorConector
		{
			public List<BlockType> tiposAceptados;
			public bool TipoEsAceptado(BlockType t)
			{
				foreach (var aceptado in tiposAceptados)
				{
					if (t.PerteneceA(aceptado)) return true;
				}
				return false;
			}
			public bool TipoEsIgual(BlockType t)
            {
				return tiposAceptados.Contains(t);
			}
		}
		public TipoAceptadoPorConector salidaDerecha;
		public TipoAceptadoPorConector salidaAbajo;
		public TipoAceptadoPorConector seccionesAceptadas;

		//[Header("Other")]
		//public bool hasPepito;
	}
}