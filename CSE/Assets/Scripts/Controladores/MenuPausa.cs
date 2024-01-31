using CSE;
using UnityEngine;
using UnityEngine.Events;

public class MenuPausa : MonoBehaviour
{
	public GameObject menu;
	public static bool Paused { get; private set; } = false;

	/// <summary>
	/// This event will be called before returning to the main menu but after the game is saved.
	/// Its intention is to let singletons connect to this to restart any values that may require it.
	/// (So that NewGame and Continue will act the same no matter the state of the game before)
	/// </summary>
	public static UnityEvent onExitLevel = new();

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
		Paused = false;
		OperacionesGameplay.Snapshot();
		MenuPartidaController.GuardarPartidaEnCurso_S();
        XAPI_Builder.CreateStatement_GameSession(false); // finishing session
        XAPI_Builder.SendAllStatements();
        onExitLevel?.Invoke(); //Reiniciar Singletons del gameplay
        GameManager.CargarEscena(GameManager.GameScene.MENU_PARTIDA);
	}
}
