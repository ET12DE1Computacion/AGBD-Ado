namespace AGBD.Test.Core;
public interface IAdo
{
    void AltaRubro(Rubro rubro);
    List<Rubro> ObtenerRubros();
    List<Producto> ObtenerProductos();
    List<Producto> ObtenerProductos(Rubro rubro);
    void AltaProducto(Producto producto);
    List<Producto> FiltrarProductos(string atributo, object valor);
}
