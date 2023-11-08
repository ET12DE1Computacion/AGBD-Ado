using et12.edu.ar.AGBD.Ado;

namespace AGBD.Test.Core;
public class AdoTest : IAdo
{
    public AdoAGBD Ado { get; set; }
    public MapRubro MapRubro { get; set; }
    public MapProducto MapProducto { get; set; }
    public AdoTest(AdoAGBD ado)
    {
        Ado = ado;
        MapRubro = new MapRubro(Ado);
        MapProducto = new MapProducto(MapRubro);
    }
    public void AltaRubro(Rubro rubro) => MapRubro.AltaRubro(rubro);
    public List<Rubro> ObtenerRubros() => MapRubro.ObtenerRubros();
    public List<Producto> ObtenerProductos() => MapProducto.ObtenerProductos();
    public List<Producto> ObtenerProductos(Rubro rubro)
        => MapProducto.ObtenerProductos(rubro);
    public void AltaProducto(Producto producto) => MapProducto.AltaProducto(producto);
    public List<Producto> FiltrarProductos(string atributo, object valor)
        => MapProducto.FilasFiltradas(atributo, valor);

}
