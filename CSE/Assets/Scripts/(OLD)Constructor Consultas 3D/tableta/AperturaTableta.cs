using System.Collections.Generic;
using UnityEngine;
using Hexstar.CSE;
using static UnityEngine.Rendering.DebugUI;

public class AperturaTableta : MonoBehaviour
{
	[SerializeField] Animator anim;
	[SerializeField] AlmacenDeBloquesSimple almacenBloques;
	[SerializeField] string over;
	[SerializeField] string open;
	bool opened = false;

	public GameObject BloqueTabletaPrefab;
	public GameObject contenedor;
	private List<GameObject> elementos = new List<GameObject>();
	public Transform zonaSpawnBloques;

	private void Awake()
	{
		if (anim == null) anim = GetComponent<Animator>();
	}

	public void OverTable(bool value)
	{
		if (MenuPausa.Paused || Boton3D.globalStop) return;
		anim.SetBool(over, value);
	}

	public void OpenTablet(bool value)
	{
		if (MenuPausa.Paused || Boton3D.globalStop) return;
		if (value) Rellenar();
		anim.SetBool(open, value);
		opened = value;
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
