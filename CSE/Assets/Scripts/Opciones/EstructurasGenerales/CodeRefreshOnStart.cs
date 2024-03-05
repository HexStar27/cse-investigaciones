using System.Collections.Generic;
using UnityEngine;

public class CodeRefreshOnStart : MonoBehaviour
{
    [SerializeField] List<Transform> objs;
    void Start()
    {
        foreach(var obj in objs)
        {
            obj.gameObject.SetActive(true);
            obj.gameObject.SetActive(false);
        }
    }
}
