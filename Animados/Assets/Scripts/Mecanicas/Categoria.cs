public class Raza
{
    public ScriptableRaza cat;

    public string Detalle(int indice)
    {
        return cat.ventajasDesventajas[indice];
    }
}
