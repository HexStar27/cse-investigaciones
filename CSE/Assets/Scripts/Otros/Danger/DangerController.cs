using UnityEngine;
using UnityEngine.Rendering;

public class DangerController : MonoBehaviour
{
	public Camera mainCam;
	[SerializeField] private Color AAAA;
	AudioSource mainMusic;
	[SerializeField, Range(-1,1)] float pitchVariation = 0.8f;

	private void OnEnable()
	{
		if (mainMusic == null) mainMusic = GameplayCycle.Instance.Get_BGM_Source();
		mainCam.backgroundColor = AAAA;
		mainMusic.pitch = pitchVariation;
	}

	private void OnDisable()
	{
		mainMusic.pitch = 1f;
		mainCam.backgroundColor = Color.black;
	}
}
