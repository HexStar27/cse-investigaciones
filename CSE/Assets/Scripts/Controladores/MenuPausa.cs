using Hexstar;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
	public GameObject menu;
	[SerializeField] private int escenaId = 1;
	public bool Paused { get; private set; } = false;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Paused = !Paused;
			menu.SetActive(Paused);
		}
	}

	public void IrAMenuPrincipal()
	{
		OperacionesGameplay.Snapshot();
		MenuPartidaController.GuardarPartidaEnCurso_S();
		GameManager.CargarEscena_S(escenaId);
	}
}
