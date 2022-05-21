using System.Collections.Generic;
using UnityEngine;
using Hexstar.CSE;

public class AperturaTableta : MonoBehaviour
{
	[SerializeField] Animator anim;

	[SerializeField] string over;
	[SerializeField] string open;
	bool opened = false;

	public GameObject BloqueTabletaPrefab;
	public ContentScaler contenedor;
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
		foreach (var elem in elementos)
		{
			Destroy(elem);
		}
		elementos.Clear();

		if (contenedor != null)
		{
			int n = AlmacenDeBloques.instancia.datosBloques.Length;
			for (int i = 0; i < n; i++)
			{
				BloqueTabletaElemento b = Instantiate(BloqueTabletaPrefab, contenedor.transform).GetComponent<BloqueTabletaElemento>();
				b.zonaSpawnBloques = zonaSpawnBloques;
				b.Inicializar(AlmacenDeBloques.instancia.datosBloques[i]);
				elementos.Add(b.gameObject);
			}
			contenedor.Actualizar();
		}
	}
}
