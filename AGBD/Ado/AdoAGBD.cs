using et12.edu.ar.AGBD.Mapeadores;
using MySql.Data.MySqlClient;
using System.Data;

namespace et12.edu.ar.AGBD.Ado;

/// <summary>
/// Clase que contiene lo necesario para conectarse a una BD MySQL y correr SF y SP
/// </summary>
public class AdoAGBD
{
    #region Propiedades
    /// <summary>
    /// Conexión Mysql
    /// </summary>
    public MySqlConnection Conexion { get; private set; }

    /// <summary>
    /// Comando del AdoAGBD
    /// </summary>
    public MySqlCommand Comando { get; set; }

    /// <summary>
    /// Adaptador del AdoAGBD
    /// </summary>
    public MySqlDataAdapter? Adaptador { get; set; }
    #endregion
    internal AdoAGBD(string cadena)
    {
        Conexion = new MySqlConnection(cadena);
        Comando = new MySqlCommand() { Connection = Conexion };
    }

    /// <summary>
    /// Abre la conexión, ejecuta el comando pasado como parámetro y cierra la conexión.
    /// </summary>
    /// <param name="comando">Comando configurado a ejecutar.</param>
    public void EjecutarComando(MySqlCommand comando)
    {
        try
        {
            Conexion.Open();
            comando.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            Conexion.Close();
        }
    }

    /// <summary>
    /// Abre la conexión, ejecuta el comando pasado como parámetro y cierra la conexión.
    /// </summary>
    /// <param name="comando">Comando configurado a ejecutar.</param>
    public async Task EjecutarComandoAsync(MySqlCommand comando)
    {
        try
        {
            await Conexion.OpenAsync();
            await comando.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {

            throw e;
        }
        finally
        {
            await Conexion.CloseAsync();
        }
    }

    /// <summary>
    /// Abre la conexión, ejecuta el comando de AdoAGBD y cierra la conexión.
    /// </summary>
    public void EjecutarComando() => EjecutarComando(Comando);

    /// <summary>
    /// Abre la conexión, ejecuta el comando y cierra la conexión asincronicamente.
    /// </summary>
    /// <returns>Tarea del método.</returns>
    public async Task EjecutarComandoAsync() => await EjecutarComandoAsync(Comando);

    /// <summary>
    /// Método que en base al nombre de un SP, devuelve la Tabla que trae el SP.
    /// </summary>
    /// <param name="nombreSP">Nombre del Stored Procedure.</param>
    /// <returns>Datatable instanciado con la tabla que devuelve el SP</returns>
    public DataTable TablaDesdeSP(string nombreSP)
    {
        Comando = new MySqlCommand(nombreSP, Conexion);
        Comando.CommandType = CommandType.StoredProcedure;
        var tabla = new DataTable();
        Adaptador = new MySqlDataAdapter(Comando);
        try
        {
            Conexion.Open();
            Adaptador.Fill(tabla);
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            Conexion.Close();
        }
        return tabla;
    }

    /// <summary>
    /// Método que en base al nombre de un SP, devuelve la Tabla que trae el SP.
    /// </summary>
    /// <param name="nombreSP">Nombre del Stored Procedure.</param>
    /// <returns>Datatable instanciado con la tabla que devuelve el SP</returns>
    public async Task<DataTable> TablaDesdeSPAsync(string nombreSP)
    {
        Comando = new MySqlCommand(nombreSP, Conexion);
        Comando.CommandType = CommandType.StoredProcedure;
        var tabla = new DataTable();
        Adaptador = new MySqlDataAdapter(Comando);
        try
        {
            await Conexion.OpenAsync();
            await Adaptador.FillAsync(tabla);
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            await Conexion.CloseAsync();
        }
        return tabla;
    }

    /// <summary>
    /// Método genérico que en base al nombre de un SP y mapeador,
    /// devuelve una colección del tipo del mapeador
    /// </summary>
    /// <typeparam name="T">Clase del mapeador donde se obtiene la Lista</typeparam>
    /// <param name="nombreSP">Nombre del Stored Procedure</param>
    /// <param name="mapeador">Clase que mapea T</param>
    /// <returns>Coleccion del tipo T</returns>
    public List<T> ColeccionDesdeSP<T>(string nombreSP, Mapeador<T> mapeador) where T : class
        => mapeador.ColeccionDesdeTabla(TablaDesdeSP(nombreSP));

    /// <summary>
    /// Método genérico que en base al nombre de un SP y mapeador,
    /// devuelve asincronicamente una colección del tipo del mapeador
    /// </summary>
    /// <typeparam name="T">Clase del mapeador donde se obtiene la Lista</typeparam>
    /// <param name="nombreSP">Nombre del Stored Procedure</param>
    /// <param name="mapeador">Clase que mapea T</param>
    /// <returns>Tarea con la Coleccion del tipo T</returns>
    public async Task<List<T>> ColeccionDesdeSPAsync<T>(string nombreSP, Mapeador<T> mapeador) where T : class
    {
        var tabla = await TablaDesdeSPAsync(nombreSP);
        return await Task<T>.Run(() => mapeador.ColeccionDesdeTabla(tabla));
    }

    private int ConfigurarSF(Action<MySqlCommand> configurarComandoSF, bool setSalida)
    {
        Comando.Parameters.Clear();
        Comando.CommandType = CommandType.StoredProcedure;
        if (setSalida)
        {
            var para = new MySqlParameter
            {
                ParameterName = "salida",
                Direction = ParameterDirection.ReturnValue
            };
            Comando.Parameters.Add(para);
            configurarComandoSF(Comando);
            return 0;
        }
        else
        {
            configurarComandoSF(Comando);

            for (int i = 0; i < Comando.Parameters.Count; i++)
            {
                if (Comando.Parameters[i].Direction == ParameterDirection.ReturnValue)
                    return i;
            }
            throw new Exception("No hay parámetro de retorno");
        }
    }

    /// <summary>
    /// Método para ejecutar una Función Almacenada
    /// </summary>
    /// <param name="nombreSF">Nombre de la Función Almacenada</param>
    /// <param name="configurarComandoSF">Método que configura el parámetro</param>
    /// <param name="setSalida">Opcion que indica si se deja un parámetro de salida por defecto</param>
    /// <returns>object que representa el escalar devuelto por la Función Almacenada</returns>
    public object EjecutarSF(string nombreSF, Action<MySqlCommand> configurarComandoSF, bool setSalida = true)
    {
        int indiceSalida = ConfigurarSF(configurarComandoSF, setSalida);
        EjecutarComando();
        return Comando.Parameters[indiceSalida].Value;
    }

    /// <summary>
    /// Método para ejecutar una Función Almacenada asincronicamente
    /// </summary>
    /// <param name="nombreSF">Nombre de la Función Almacenada</param>
    /// <param name="configurarComandoSF">Método que configura el parámetro</param>
    /// <param name="setSalida">Opcion que indica si se deja un parámetro de salida por defecto</param>
    /// <returns>Tarea el con object que representa el escalar devuelto por la Función Almacenada</returns>
    public async Task<object> EjecutarSFAsync(string nombreSF, Action<MySqlCommand> configurarComandoSF, bool setSalida = true)
    {
        int indiceSalida = ConfigurarSF(configurarComandoSF, setSalida);
        await EjecutarComandoAsync();
        return Comando.Parameters[indiceSalida].Value;
    }

    internal DataTable TablaPorComando(MySqlCommand comando)
    {
        var tabla = new DataTable();
        Adaptador = new MySqlDataAdapter(comando);
        Adaptador.Fill(tabla);
        return tabla;
    }

    internal async Task<DataTable> TablaPorComandoAsync(MySqlCommand comando)
    {
        var tabla = new DataTable();
        Adaptador = new MySqlDataAdapter(comando);
        await Adaptador.FillAsync(tabla);
        return tabla;
    }

}