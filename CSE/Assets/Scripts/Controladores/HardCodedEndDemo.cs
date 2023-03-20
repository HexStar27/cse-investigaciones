using UnityEngine;

public class HardCodedEndDemo : MonoBehaviour
{
	public GameObject mensajeFinAlfa;
	bool esperando = true;
	public void CheckWin()
	{
		if(ResourceManager.CasosCompletados > 0)
		{
			mensajeFinAlfa.SetActive(true);
			esperando = false;
		}
	}

	private void FixedUpdate()
	{
		if(esperando)
		{
			CheckWin();
		}
	}
}
