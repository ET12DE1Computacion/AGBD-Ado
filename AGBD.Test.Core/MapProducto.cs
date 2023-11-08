using et12.edu.ar.AGBD.Ado;
using et12.edu.ar.AGBD.Mapeadores;
using System.Data;

namespace AGBD.Test.Core;
public class MapProducto : Mapeador<Producto>
{
    public MapRubro MapRubro { get; set; }
    public MapProducto(AdoAGBD ado) : base(ado) => Tabla = "Producto";
    public MapProducto(MapRubro mapRubro) : this(mapRubro.AdoAGBD)
        => MapRubro = mapRubro;
    public override Producto ObjetoDesdeFila(DataRow fila) => new Producto()
    {
        Nombre = fila["nombre"].ToString(),
        Cantidad = Convert.ToUInt16(fila["cantidad"]),
        PrecioUnitario = Convert.ToDecimal(fila["precioUnitario"]),
        Rubro = MapRubro.FiltrarPorPK("idRubro", Convert.ToByte(fila["idRubro"])),
        Id = Convert.ToInt16(fila["idProducto"])
    };

    public List<Producto> ObtenerProductos() => ColeccionDesdeTabla();
    public List<Producto> ObtenerProductos(Rubro rubro)
    {
        SetComandoSP("ProductosPorRubro");

        BP.CrearParametro("unIdRubro")
          .SetTipo(MySql.Data.MySqlClient.MySqlDbType.UByte)
          .SetValor(rubro.Id)
          .AgregarParametro();

        return ColeccionDesdeSP();
    }

    public void AltaProducto(Producto producto)
      => EjecutarComandoCon("altaProducto", ConfigurarAltaProducto, PostAltaProducto, producto);

    private void ConfigurarAltaProducto(Producto producto)
    {
        SetComandoSP("altaProducto");

        BP.CrearParametroSalida("unIdProducto")
          .SetTipo(MySql.Data.MySqlClient.MySqlDbType.Int16)
          .AgregarParametro();

        BP.CrearParametro("unIdRubro")
          .SetTipo(MySql.Data.MySqlClient.MySqlDbType.UByte)
          .SetValor(producto.Rubro.Id)
          .AgregarParametro();

        BP.CrearParametro("unNombre")
          .SetTipoVarchar(45)
          .SetValor(producto.Nombre)
          .AgregarParametro();

        BP.CrearParametro("unPrecioUnitario")
          .SetTipoDecimal(7, 2)
          .SetValor(producto.PrecioUnitario)
          .AgregarParametro();

        BP.CrearParametro("unaCantidad")
          .SetTipo(MySql.Data.MySqlClient.MySqlDbType.UInt16)
          .SetValor(producto.Cantidad)
          .AgregarParametro();
    }
    private void PostAltaProducto(Producto producto)
        => producto.Id = Convert.ToInt16(GetParametro("unIdProducto").Value);
}
