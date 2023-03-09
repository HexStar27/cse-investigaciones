using UnityEngine;
using UnityEngine.UI;

public class DelayedDeleteAtStart : MonoBehaviour
{
    public float delay = 1;
    private void Awake()
    {
        GetComponent<Image>().enabled = true;
    }
    void Start()
    {
        Destroy(gameObject, delay);
    }
}
