using UnityEngine;

public class CanceladorDeMovimientoDeCamara : MonoBehaviour
{
    public void Cancelar(bool value)
    {
        InscryptionLikeCameraState.bypass = value;
    }
}
