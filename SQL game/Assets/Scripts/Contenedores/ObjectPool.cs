using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	[SerializeField] private GameObject contenedor;
	[SerializeField] private GameObject prefab;
	[SerializeField] private uint initialSize;
	[SerializeField] private List<GameObject> pool;
	private void Start()
	{
		pool = GenerateObjs(initialSize);
	}

	private List<GameObject> GenerateObjs(uint amount)
	{
		for (uint i = 0; i < amount; i++)
		{
			GameObject thing = Instantiate(prefab);
			thing.transform.parent = contenedor.transform;
			thing.SetActive(false);
			pool.Add(thing);
		}
		return pool;
	}

	/// <summary>
	/// Pasa un objeto almacenado en la Pool de objetos que no esté en uso.
	/// Si no hay ningún objeto libre creará uno nuevo y lo añadirá a la Pool de objetos.
	/// </summary>
	/// <returns>El objeto pedido</returns>
	public GameObject RequestObj()
	{
		foreach (GameObject thing in pool)
		{
			if (thing.activeInHierarchy == false)
			{
				thing.SetActive(true);
				return thing;
			}
		}
		//Si no quedan más cosas en la Pool, se generan más de forma dinámica :)
		GameObject newThing = Instantiate(prefab);
		newThing.transform.parent = contenedor.transform;
		pool.Add(newThing);
		return newThing;
	}

	//Los objetos deben desactivarse cuando normalmente se destruían para que el Pool los pueda reciclar.
}
