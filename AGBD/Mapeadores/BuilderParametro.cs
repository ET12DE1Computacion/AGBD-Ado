using MySql.Data.MySqlClient;
using System.Data;

namespace et12.edu.ar.AGBD.Mapeadores;

/// <summary>
/// Clase para asistir en la generación y configuración de parámetros de MySQLCommand.
/// </summary>
public class BuilderParametro
{
    /// <summary>
    /// Parámetro que usa la clase para configurar y devolver.
    /// </summary>
    public MySqlParameter Parametro { get; private set; }
    private IMapConParametros Mapeador {get; set;}
    internal BuilderParametro(IMapConParametros maper) => Mapeador = maper;

    /// <summary>
    /// Método que instancia a Parametro. Por defecto la dirección es Input y valor DBNull
    /// </summary>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro CrearParametro()
    {
        Parametro = new MySqlParameter()
        {
            Direction = ParameterDirection.Input,
            Value = DBNull.Value
        };
        return this;
    }

    /// <summary>
    /// Método que instancia a Parametro con nombre. Por defecto la dirección es Input y valor DBNull
    /// </summary>
    /// <param name="nombre">Nombre del parámetro</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro CrearParametro(string nombre)
    {
        CrearParametro();
        return SetNombre(nombre);
    }

    /// <summary>
    /// Método que instancia a Parametro con dirección Output
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro CrearParametroSalida(string nombre)
        => CrearParametro(nombre)
          .SetDireccion(ParameterDirection.Output)
          .SetValor(null);

    /// <summary>
    /// Método que setea un nombre al Parametro.
    /// </summary>
    /// <param name="nombre">Nombre a setear.</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro SetNombre(string nombre)
    {
        Parametro.ParameterName = nombre;
        return this;
    }

    /// <summary>
    /// Método que setea una dirección al Parametro.
    /// </summary>
    /// <param name="direccion">Dirección a setear.</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro SetDireccion(ParameterDirection direccion)
    {
        Parametro.Direction = direccion;
        return this;
    }

    /// <summary>
    /// Método que setea un valor al Parametro. Si valor es nulo, setea DBNull.
    /// </summary>
    /// <param name="valor">Valor a setear.</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro SetValor(object valor)
    {
        Parametro.Value = valor ?? DBNull.Value;
        return this;
    }

    /// <summary>
    /// Método que setea un el tipo de dato al Parametro.
    /// </summary>
    /// <param name="tipo">Tipo a setear.</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro SetTipo(MySqlDbType tipo)
    {
        Parametro.MySqlDbType = tipo;
        return this;
    }

    /// <summary>
    /// Método que setea el tipo Decimal al Parametro.
    /// </summary>
    /// <param name="precision">Cantidad de dígitos en total.</param>
    /// <param name="escala">Cantidad de dígitos detrás de la coma.</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro SetTipoDecimal(byte precision, byte escala)
    {
        Parametro.Precision = precision;
        Parametro.Scale = escala;
        return SetTipo(MySqlDbType.Decimal);
    }

    /// <summary>
    /// Método que setea el tipo VARCHAR(n) al Parametro.
    /// </summary>
    /// <param name="longitud">Cantidad de caracteres máximos</param>
    /// <returns>BuilderParametro para encadenar con otros métodos.</returns>
    public BuilderParametro SetTipoVarchar(int longitud)
    {
        Parametro.Size = longitud;
        return SetTipo(MySqlDbType.VarChar);
    }

    /// <summary>
    /// Método que setea el tipo CHAR(n) al Parametro.
    /// </summary>
    /// <param name="longitud">Cantidad de caracteres de longitud fija.</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro SetTipoChar(int longitud)
    {
        Parametro.Size = longitud;
        return SetTipo(MySqlDbType.String);
    }

    /// <summary>
    /// Método que setea la longitud al Parametro.
    /// </summary>
    /// <param name="longitud">Longitud del Parametro</param>
    /// <returns>BuilderParametro para encadenar con otros métodos</returns>
    public BuilderParametro SetLongitud(int longitud)
    {
        Parametro.Size = longitud;
        return this;
    }

    /// <summary>
    /// Método que agrega el parámetro en construcción al mapeador asociado.
    /// </summary>
    /// <returns>Devuelve el parametro construido</returns>
    public MySqlParameter AgregarParametro()
    {
        Mapeador.AgregarParametro(Parametro);
        return Parametro;
    }
}