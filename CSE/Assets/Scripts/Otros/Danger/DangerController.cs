using UnityEngine;
using UnityEngine.Rendering;

public class DangerController : MonoBehaviour
{
	public Camera mainCam;
	[SerializeField] private Color AAAA;
	[SerializeField] AudioSource mainMusic;
	[SerializeField, Range(-1,1)] float pitchVariation = 0.8f;

	private void OnEnable()
	{
		mainCam.backgroundColor = AAAA;
		mainMusic.pitch = pitchVariation;
	}

	private void OnDisable()
	{
		mainMusic.pitch = 1f;
		mainCam.backgroundColor = Color.black;
	}
}
