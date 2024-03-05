using UnityEngine;

public class DangerController : MonoBehaviour
{
	public Camera mainCam;
	[SerializeField] private Color AAAA, cambioLuces;
	AudioSource mainMusic;
	[SerializeField, Range(-1,1)] float pitchVariation = 0.8f;
	[Space()]
	public Light[] lights;
	private Color colorAntiguo;

	private void OnEnable()
	{
		if (mainMusic == null) mainMusic = GameplayCycle.Instance.Get_BGM_Source();
		mainCam.backgroundColor = AAAA;
		if (lights != null && lights.Length > 0)
		{
			colorAntiguo = lights[0].color;
			foreach (var l in lights) l.color = cambioLuces;
		}
		mainMusic.pitch = pitchVariation;
	}

	private void OnDisable()
	{
		mainMusic.pitch = 1f;
		mainCam.backgroundColor = Color.black;
		if (lights != null && lights.Length > 0)
		{
			foreach (var l in lights) l.color = colorAntiguo;
		}
	}
}
