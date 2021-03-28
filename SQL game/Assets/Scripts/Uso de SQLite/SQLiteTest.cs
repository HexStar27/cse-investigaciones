using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class SQLiteTest : MonoBehaviour
{
	public string dbName = "CSE_Database";

	private string connectionPath;	//Ruta de la BD
	private IDbConnection dbConnection;//Para la conexión 

	private void Awake()
	{
		connectionPath = "URI=file:" + Application.persistentDataPath + "/" + dbName;
		dbConnection = new SqliteConnection(connectionPath);
	}

	private void OnEnable()
	{
		dbConnection.Open();
	}

	private void OnDisable()
	{
		dbConnection.Close();
	}


	public void OpenConection()
	{
		dbConnection.Open();
	}

	public void CloseConection()
	{
		dbConnection.Close();
	}


	/// <summary>
	/// Ejecuta la consulta pasada por parámetro en la base de datos local.
	/// La conexión con la base de datos debe de estar abierta.
	/// </summary>
	/// <param name="query">Consulta de lectura de una base de datos (select...)</param>
	/// <returns>Un lector de datos que contiene el array del resultado de la consulta</returns>
	public IDataReader ExecuteQuery(string query)
	{
		IDbCommand dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = query;
		return dbCommand.ExecuteReader();
	}

	/// <summary>
	/// Ejecuta la consulta pasada por parámetro en la base de datos local.
	/// La conexión con la base de datos debe de estar abierta.
	/// </summary>
	/// <param name="query">Consulta de tipo CREATE, UPDATE, DELETE, INSERT, ...</param>
	/// <returns>Devuelve el código de estado de la operación al finalizar</returns>
	public int ModifyDB (string query)
	{
		IDbCommand dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = query;
		return dbCommand.ExecuteNonQuery();
	}

	/// <summary>
	/// Ejecuta la consulta pasada por parámetro en la base de datos local.
	/// La conexión con la base de datos debe de estar abierta.
	/// </summary>
	/// <param name="query">Consulta de función cuyo resultado será de 1 sólo valor, ej: las funciones de fecha y las de operaciones matemáticas</param>
	/// <returns>El valor de la consulta</returns>
	public object ExecuteScalarQuery(string query)
	{
		IDbCommand dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = query;
		return dbCommand.ExecuteScalar();
	}
}



/*
	 ExecuteNonQuery():
		will work with Action Queries only (Create,Alter,Drop,Insert,Update,Delete).
		Returns the count of rows effected by the Query.
		Return type is int
		Return value is optional and can be assigned to an integer variable.
	
	ExecuteReader():
		will work with Action and Non-Action Queries (Select)
		Returns the collection of rows selected by the Query.
		Return type is DataReader.
		Return value is compulsory and should be assigned to an another object DataReader.
	
	ExecuteScalar():
		will work with Non-Action Queries that contain aggregate functions.
		Return the first row and first column value of the query result.
		Return type is object.
		Return value is compulsory and should be assigned to a variable of required type.
*/
