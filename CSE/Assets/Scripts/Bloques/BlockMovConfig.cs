using UnityEngine;

namespace Hexstar.CSE
{
	[CreateAssetMenu(fileName = "BlockConfig", menuName = "Block/Block Movement Configuration")]
	public class BlockMovConfig : ScriptableObject
	{
		public GameObject ghostBlock = null;
		public GameObject deletionBlock = null;

		[Header("Grabbing related")]
		public float deletionThreshold = 0.2f;

		public LayerMask suelo;
		public float altura = 0;
		public float grabHeight = 0;
		public Vector3 centerLBox;
		public Vector3 sizeLBox;

		[Header("Audio Configuration")]
		public AudioClip blockSelect;
		public AudioClip blockConect, blockDisconect;
	}
}