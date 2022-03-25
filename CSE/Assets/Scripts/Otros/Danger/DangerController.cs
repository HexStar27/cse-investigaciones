using UnityEngine;
using UnityEngine.Rendering;

public class DangerController : MonoBehaviour
{
	public Camera mainCam;
	[SerializeField] private Color AAAA;

	private void OnEnable()
	{
		mainCam.backgroundColor = AAAA;
	}

	private void OnDisable()
	{
		mainCam.backgroundColor = Color.black;
	}
}
