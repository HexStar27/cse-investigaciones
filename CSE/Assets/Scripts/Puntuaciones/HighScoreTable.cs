using UnityEngine;
using Hexstar;
using SimpleJSON;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;

public class HighScoreTable : MonoBehaviour
{
    GameObject scoreRowPrefab;
    readonly Color primero = new Color(.95f, 1, 0, 1);
    readonly Color segundo = new Color(1, 0, .5f, 1);

    //If caso < 0, usa general, else la puntuacion del caso especificado
    [SerializeField] private int caso = -1 ;
    [HideInInspector] public List<GameObject> elements = new List<GameObject>();
    [SerializeField] bool loadOnStart = false;

    private void Awake()
    {
        InitializePrefab();
    }

    public void InitializePrefab()
    {
        scoreRowPrefab = Resources.Load("PuntuacionHighScorePrefab") as GameObject;
    }

    private void Start()
    {
        if (loadOnStart) _ = SetupScore();
    }

    public async Task SetupScore(bool deletePrevious = true)
    {
        WWWForm form = new WWWForm();
        form.AddField("caso", caso);
        await ConexionHandler.APost(ConexionHandler.baseUrl + "score", form);
        if(deletePrevious) DeleteElements();
        GetData(ConexionHandler.download);
    }

    public void SetCasoID(int id) { caso = id; }

    private void GetData(string download)
    {
        JSONNode json = JSON.Parse(download);
        if(json == null)
        {
            Debug.LogError("No se ha podido recivir nada del servidor :(");
            return;
        }
        if (json.HasKey("info"))
        {
            if (json["info"].Value != "Correcto" || !json.HasKey("res"))
            {
                Debug.LogError("El servidor a avisado de que algo ha ido mal :(");
                Debug.LogWarning(json["info"].Value);
            }
            else
            {
                int n = json["res"].Count;
                for (int i = 0; i < n; i++)
                {
                    GameObject row = Instantiate(scoreRowPrefab, transform);
                    if (row.TryGetComponent(out TextMeshProUGUI tmp)) {
                        tmp.text = json["res"][i]["nickname"].Value; 
                    }
                    else Debug.Log("Ups no se pudo obtener el nickname");
                    if(row.transform.GetChild(0).TryGetComponent(out tmp))
                    {
                        tmp.text = json["res"][i]["score"].Value;
                        if (i == 0) tmp.color = primero;
                        else if (i == 1) tmp.color = segundo;
                    }
                    else Debug.Log("Ups no se pudo obtener la score");
                    elements.Add(row);
                }
            }
        }
    }
    public void DeleteElements()
    {
        int n = elements.Count;
        for (int i = n - 1; i >= 0; i--)
            Destroy(elements[i]);
        elements.Clear();
    }

    public void ShowOnlyRange(int from, int to)
    {
        int n = elements.Count;
        for(int i = 0; i < n; i++) elements[i].SetActive(i >= from && i < to);
    }
}
