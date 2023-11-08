namespace AGBD.Test.Core;

public class Servicio
{
    private readonly IAdo _ado;
    public Servicio(IAdo ado) => _ado = ado;

    public void AltaProducto(Producto producto)
    {
        // Verififcaciones
        _ado.AltaProducto(producto);
    }

    public void AltaRubro(Rubro rubro)
    {
        throw new NotImplementedException();
    }

    public List<Producto> FiltrarProductos(string atributo, object valor)
    {
        throw new NotImplementedException();
    }

    public List<Producto> ObtenerProductos()
    {
        throw new NotImplementedException();
    }

    public List<Producto> ObtenerProductos(Rubro rubro)
    {
        throw new NotImplementedException();
    }

    public List<Rubro> ObtenerRubros() => _ado.ObtenerRubros();
}
