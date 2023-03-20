using UnityEngine;
using Hexstar;

public class ConsultaTest : MonoBehaviour
{
    public string consulta;
    [TextArea(1,100)]
    public string resultado;

    private void OnEnable()
    {
        Consultar();
    }

    public async void Consultar()
    {
        WWWForm form = new WWWForm();
        form.AddField("consulta",consulta);

        await ConexionHandler.APost(ConexionHandler.baseUrl+"case/check",form);
        resultado = ConexionHandler.download;
    }
}
