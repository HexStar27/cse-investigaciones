using System.Collections.Generic;
using UnityEngine;

public class BlockModelControl : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private GameObject top, left, left2, bot, bot2, right;
#pragma warning restore 0649
	public void Activar(bool u, bool l, bool d, bool r)
	{
		ActivarTop(u);
		ActivarLeft(l);
		ActivarBottom(d);
		ActivarRight(r);
	}

	public void ActivarTop(bool v) { top.SetActive(v); }
	public void ActivarLeft(bool v) { left.SetActive(v); left2.SetActive(!v); }
	public void ActivarBottom(bool v) { bot.SetActive(v); bot2.SetActive(!v); }
	public void ActivarRight(bool v) { right.SetActive(v); }

	public List<GameObject> GetActivados()
	{
		List<GameObject> activados = new List<GameObject>();
		if (top.activeSelf)		activados.Add(top);
		if (left.activeSelf)	activados.Add(left);
		if (bot.activeSelf)		activados.Add(bot);
		if (right.activeSelf)	activados.Add(right);
		return activados;
	}
}
