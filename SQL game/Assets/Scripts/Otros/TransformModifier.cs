using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformModifier : MonoBehaviour
{
    public void EjeX(int x) { transform.position = new Vector3(x, transform.position.y, transform.position.z); }
    public void EjeY(int y) { transform.position = new Vector3(transform.position.x, y, transform.position.z); }
    public void EjeZ(int z) { transform.position = new Vector3(transform.position.x, transform.position.y, z); }

}
