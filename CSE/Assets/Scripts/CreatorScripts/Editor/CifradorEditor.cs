using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PassCreator))]
public class CifradorEditor : Editor
{
	private PassCreator script;

	private void OnEnable()
	{
		// Method 1
		script = (PassCreator)target;
	}

	public override void OnInspectorGUI()
	{
		// Draw default inspector after button...
		base.OnInspectorGUI();

		if (GUILayout.Button("Cifrar"))
		{
			Hexstar.SesionHandler.ciphKey = script.key;
			Hexstar.SesionHandler.ciphIv = script.iv;

			List<Dictionary<string,object>> data = CSVReader.Read (script.pathToCSV);
			int n = 0;
			if (data == null)
			{
				n = script.texto.Length;
				script.cifrado = new string[n];
				for (int i = 0; i < n; i++)
				{
					script.cifrado[i] = Hexstar.SesionHandler.Cifrar(script.texto[i]);
				}
			}
			else
			{
				n = data.Count;
				script.cifrado = new string[n];
				for (int i = 0; i < n; i++)
				{
					script.cifrado[i] = Hexstar.SesionHandler.Cifrar((string)data[i]["password"]);
				}
			}
		}

		if (GUILayout.Button("Save"))
		{		
			string path = "Assets/Resources/"+script.pathToCSV+"_cifrado.txt";

			//Write some text to the test.txt file
			StreamWriter writer = new StreamWriter(path, true);
			int n = script.cifrado.Length;
			for (int i = 0; i < n; i++)
			{
				writer.WriteLine(script.cifrado[i]);
			}
			writer.Close();
		}
	}
}
