using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar
{
	class BancoEventos:MonoBehaviour
	{

		protected Dictionary<string, object> banco = new Dictionary<string, object>();

		private static BancoEventos instance_;
		public static BancoEventos instance() { return instance_; }

		public void Awake()
		{
			instance_ = this;
		}

		/// <summary>
		/// Get the value saved in the event
		/// </summary>
		/// <param name="eventName">name of the event</param>
		/// <returns>value</returns>
		public object Get(string eventName)
		{
			if(banco.TryGetValue(eventName, out object value)) return value;
			else return null;
		}

		/// <summary>
		/// Set the value of a new or already existing event
		/// </summary>
		/// <param name="eventName">name of event</param>
		/// <param name="value">value of event</param>
		public void Set(string eventName, object value = null)
		{
			if (banco.ContainsKey(eventName)) banco[eventName] = value;
			else banco.Add(eventName, value);
		}

		/// <summary>
		/// Delete the event from the bank
		/// </summary>
		/// <param name="eventName">name of event</param>
		/// <returns>Whether the event was found or not</returns>
		public bool Delete(string eventName)
		{
			return banco.Remove(eventName);
		}

		/// <summary>
		/// Save the data of the bank in a file (possibly in /Documents xD)
		/// </summary>
		/// <param name="fileName">name of the file :v</param>
		/// <returns>Whether was possible to save into a file or not</returns>
		public bool Save(string fileName)
		{
			//Implementar algo pa que guarde en los documentos los valores de cada evento (probablemente serializado o algo)

			return false;
		}

		/// <summary>
		/// Delete all event in the bank
		/// </summary>
		public void DeleteAll()
		{
			banco.Clear();
		}

	}
}