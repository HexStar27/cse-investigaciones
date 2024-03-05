using System.Collections.Generic;
using UnityEngine;
using Hexstar.CSE;

public class AperturaTableta : MonoBehaviour
{
	[SerializeField] Animator anim;
	[SerializeField] AlmacenDeBloquesSimple almacenBloques;
	[SerializeField] string over;
	[SerializeField] string open;
	bool opened = false;

	public GameObject BloqueTabletaPrefab;
	public GameObject contenedor;
	private List<GameObject> elementos = new();
	public Transform zonaSpawnBloques;

	private CameraStalker cs;

	private void Awake()
	{
		if (anim == null) anim = GetComponent<Animator>();
		cs = GetComponent<CameraStalker>();
	}

	public void OverTable(bool value)
	{
		if (MenuPausa.Paused || Boton3D.globalStop) return;
		anim.SetBool(over, value);
	}

	public void OpenTablet(bool value)
	{
		bool farOpening = false;
		if(value) farOpening = cs.CorrectState();
        if (MenuPausa.Paused || Boton3D.globalStop || farOpening) return;
		if (value) Rellenar();
		anim.SetBool(open, value);
		opened = value;
		if(opened) cs.onCameraReady.AddListener(TooFarFromTablet);
    }
	private void TooFarFromTablet()
	{
		OpenTablet(false);
        cs.onCameraReady.RemoveListener(TooFarFromTablet);
    }
	public void ForceClose()
	{
        opened = false;
    }
	public void SwitchStateTablet() { OpenTablet(!opened); }


	public void Rellenar()
	{
		foreach (var elem in elementos) Destroy(elem);
		elementos.Clear();

		var bloques = almacenBloques.BloquesDisponiblesEnDificultad(ResourceManager.DificultadActual);
		foreach(var bloque in bloques)
        {
			BloqueTabletaElemento bte = Instantiate(BloqueTabletaPrefab, contenedor.transform).GetComponent<BloqueTabletaElemento>();
			var rt = bte.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0, 1);
			rt.anchorMax = new Vector2(1, 1);
			bte.zonaSpawnBloques = zonaSpawnBloques;
			bte.Inicializar(bloque);
			elementos.Add(bte.gameObject);
		}
	}
}
