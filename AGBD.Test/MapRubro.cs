using et12.edu.ar.AGBD.Mapeadores;
using et12.edu.ar.AGBD.Ado;
using System.Data;

namespace AGBD.Test;

public class MapRubro : Mapeador<Rubro>
{
    public MapRubro(AdoAGBD ado) : base(ado) => Tabla = "Rubro";
    public override Rubro ObjetoDesdeFila(DataRow fila)
        => new Rubro()
        {
            Id = Convert.ToByte(fila["idRubro"]),
            Nombre = fila["rubro"].ToString()
        };
    public void AltaRubro(Rubro rubro)
        => EjecutarComandoCon("altaRubro", ConfigurarAltaRubro, PostAltaRubro, rubro);
    public void ConfigurarAltaRubro(Rubro rubro)
    {
        SetComandoSP("altaRubro");

        BP.CrearParametroSalida("unIdRubro")
          .SetTipo(MySql.Data.MySqlClient.MySqlDbType.UByte)
          .AgregarParametro();

        BP.CrearParametro("unRubro")
          .SetTipoVarchar(45)
          .SetValor(rubro.Nombre)
          .AgregarParametro();
    }

    internal Rubro RubroPorId(byte id)
    {
        var rubros = FilasFiltradas("idRubro", id);

        return rubros[0];
    }

    public void PostAltaRubro(Rubro rubro)
    {
        var paramIdRubro = GetParametro("unIdRubro");
        rubro.Id = Convert.ToByte(paramIdRubro.Value);
    }

    public List<Rubro> ObtenerRubros() => ColeccionDesdeTabla();
}