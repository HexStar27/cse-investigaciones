﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar
{
	class BancoEventos:MonoBehaviour
	{

		protected List<Evento> banco = new List<Evento>();
		protected List<Evento> activos = new List<Evento>();

		private static BancoEventos instance_;
		public static BancoEventos Instance() { return instance_; }

		public void Awake()
		{
			instance_ = this;
		}

		/// <summary>
		/// Get the value saved in the event
		/// </summary>
		/// <param name="eventName">name of the event</param>
		/// <returns>value</returns>
		public Evento Get(int eventIndex)
		{
			return banco[eventIndex];
		}

		/// <summary>
		/// Set the value of a new or already existing event
		/// </summary>
		/// <param name="eventName">name of event</param>
		/// <param name="value">value of event</param>
		public void Add( Evento newEvent)
		{
			banco.Add(newEvent);
		}

		public void Activate(Evento e)
		{

			activos.Add(e);
		}

		/// <summary>
		/// Delete all events in the bank
		/// </summary>
		public void Clear()
		{
			banco.Clear();
		}

		public int Count()
		{
			return banco.Count;
		}
	}
}