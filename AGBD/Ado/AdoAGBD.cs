using et12.edu.ar.AGBD.Mapeadores;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace et12.edu.ar.AGBD.Ado
{
    /// <summary>
    /// Clase que contiene lo necesario para conectarse a una BD MySQL y correr SF y SP
    /// </summary>
    public class AdoAGBD
    {
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
        public MySqlDataAdapter Adaptador { get; set; }
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
                Conexion.Close();
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
        /// Abre la conexión, ejecuta el comando de AdoAGBD y cierra la conexión.
        /// </summary>
        public void EjecutarComando() => EjecutarComando(Comando);

        /// <summary>
        /// Método que en base al nombre de un SP, devuelve la Tabla que trae el SP.
        /// </summary>
        /// <param name="nombreSP">Nombre del Stored Procedure.</param>
        /// <returns>Datatable instanciado con la tabla que devuelve el SP</returns>
        public DataTable TablaDesdeSP (string nombreSP)
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
        /// Método genérico que en base al nombre de un SP y mapeador,
        /// devuelve una colección del tipo del mapeador
        /// </summary>
        /// <typeparam name="T">Clase del mapeador donde se obtiene la Lista</typeparam>
        /// <param name="nombreSP">Nombre del Stored Procedure</param>
        /// <param name="mapeador">Clase que mapea T</param>
        /// <returns>Coleccion del tipo T</returns>
        public List<T> ColeccionDesdeSP<T> (string nombreSP, Mapeador<T> mapeador)
            => mapeador.ColeccionDesdeTabla(TablaDesdeSP(nombreSP));
    
        /// <summary>
        /// Método para ejecutar una Función Almacenada
        /// </summary>
        /// <param name="nombreSF">Nombre de la Función Almacenada</param>
        /// <param name="configurarComandoSF">Método que configura el parámetro</param>
        /// <param name="setSalida">Opcion que indica si se deja un parámetro de salida por defecto</param>
        /// <returns>object que representa el escalar devuelto por la Función Almacenada</returns>
        public object EjecutarSF (string nombreSF, Action<MySqlCommand> configurarComandoSF, bool setSalida = true)
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
            }
            configurarComandoSF(Comando);
            
            int indiceSalida=0;
            int i;
            for (i = 0; i < Comando.Parameters.Count; i++)
            {
                if (Comando.Parameters[i].Direction == ParameterDirection.ReturnValue)
                {
                    indiceSalida = i;
                    break;
                }
            }
            if (i == Comando.Parameters.Count)
            {
                throw new Exception("No hay parámetro de retorno");
            }
            EjecutarComando();
            return Comando.Parameters[indiceSalida].Value;
        }

        internal DataTable TablaPorComando(MySqlCommand comando)
        {
            var tabla = new DataTable();
            Adaptador = new MySqlDataAdapter(comando);
            Adaptador.Fill(tabla);
            return tabla;
        }

        /// <summary>
        /// Abre la conexión, ejecuta el comando y cierra la conexión asincronicamente.
        /// </summary>
        /// <returns>Tarea del método.</returns>
        public async Task EjecutarComandoAsync()
        {
            try
            {
                await Conexion.OpenAsync().ConfigureAwait(false);
                await Comando.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw ;
            }
            finally
            {
                await Conexion.CloseAsync();
            }
        }
    }
}