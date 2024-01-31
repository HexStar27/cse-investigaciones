using UnityEngine;

public class HardCodedEndDemo : MonoBehaviour
{
	public GameObject mensajeFinAlfa;
	bool esperando = true;
	public int casosNecesarios = 2;
	public void CheckWin()
	{
		if(ResourceManager.CasosCompletados.Count >= casosNecesarios)
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
