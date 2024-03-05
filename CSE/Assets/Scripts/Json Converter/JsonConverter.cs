using UnityEngine;

namespace Hexstar
{
    public class JsonConverter
    {
        /// <summary>
        /// Crea un json con los datos del objeto.
        /// </summary>
        /// <typeparam name="T">Clase serializable</typeparam>
        /// <param name="objeto">Objeto a convertir</param>
        /// <returns>Json en forma de cadena</returns>
        public static string ConvertirAJson<T>(T objeto)
        {
            return JsonUtility.ToJson(objeto);
        }

        /// <summary>
        /// Crea un objeto con los datos del json.
        /// </summary>
        /// <typeparam name="T">Clase serializable</typeparam>
        /// <param name="json">El json</param>
        /// <returns>Objeto con contenido del json</returns>
        public static T PasarJsonAObjeto<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// Sobrescribe un objeto con los datos del json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">El json</param>
        /// <param name="objeto">Objeto inicializado</param>
        public static void PasarJsonAObjeto<T>(string json, T objeto)
        {
            JsonUtility.FromJsonOverwrite(json, objeto);
            
        }
    }
}