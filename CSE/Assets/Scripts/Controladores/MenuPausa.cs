using Hexstar;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
	public GameObject menu;
	[SerializeField] private int escenaId = 1;
	public static bool Paused { get; private set; } = false;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape)) Pausar();
	}

	public void Pausar()
    {
		Paused = !Paused;
		menu.SetActive(Paused);
		if (Paused) GameManager.OnPause.Invoke();
		else GameManager.OnUnpause.Invoke();
	}

	public void IrAMenuPrincipal()
	{
		OperacionesGameplay.Snapshot();
		MenuPartidaController.GuardarPartidaEnCurso_S();
		GameManager.CargarEscena(escenaId);
	}
}
