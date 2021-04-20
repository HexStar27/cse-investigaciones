
using UnityEngine;

public interface IListaUIElement
{
    void ActualizarPosicionLocal(Vector3 localPos);

    void AsignarPadre(Transform t);

    void AsignarLista(ListaUI lui);

    void DesreferenciarLista();
}
