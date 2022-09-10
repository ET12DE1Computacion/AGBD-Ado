using Microsoft.Extensions.Configuration;

namespace et12.edu.ar.AGBD.Ado;

/// <summary>
/// Clase que abstrae de la creación de la conexión de <c>AdoAGBD</c>
/// </summary>
public static class FactoryAdoAGBD
{
    /// <summary>
    /// Método que devuelve una instancia de <c>AdoAGBD</c>
    /// </summary>
    /// <param name="cadena">Cadena de conexión plana</param>
    /// <returns>Instancia de <c>AdoAGBD</c></returns>
    public static AdoAGBD GetAdoMySQL(string cadena)
        => new AdoAGBD(cadena);

    /// <summary>
    /// Método que devuelve una instancia de <c>AdoAGBD</c>.
    /// </summary>
    /// <param name="archivo">Archivo <c>.json</c> con ruta.</param>
    /// <param name="nombreConexion">Nombre de la conexión.</param>
    /// <returns>Instancia de <c>AdoAGBD</c></returns>
    public static AdoAGBD GetAdoMySQL(string archivo, string nombreConexion)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(archivo, optional: true, reloadOnChange: true)
            .Build();
        string cadena = config.GetConnectionString(nombreConexion);
        return new AdoAGBD(cadena);
    }
}