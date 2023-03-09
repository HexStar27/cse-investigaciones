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
	private List<GameObject> elementos = new List<GameObject>();
	public Transform zonaSpawnBloques;

	private void Awake()
	{
		if (anim == null) anim = GetComponent<Animator>();
	}

	public void OverTable(bool value)
	{
		anim.SetBool(over, value);
	}

	public void OpenTablet(bool value)
	{
		if (value) Rellenar();
		anim.SetBool(open, value);
		opened = value;
	}
	public void SwitchStateTablet()
	{
		opened = !opened;
		if (opened) Rellenar();
		anim.SetBool(open, opened);
	}


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

		//if (contenedor.TryGetComponent(out ContentScaler cs)) cs.Actualizar();
	}
}
