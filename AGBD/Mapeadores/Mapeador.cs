using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using et12.edu.ar.AGBD.Ado;

namespace et12.edu.ar.AGBD.Mapeadores
{
    /// <summary>
    /// Clase abstracta y genérica para facilitar las configuraciones del mapeo de clases
    /// </summary>
    /// <typeparam name="T">El elemento sobre el que va a trabajar el Mapeador</typeparam>
    public abstract class Mapeador<T> : IMapConParametros
    {
        /// <summary>
        /// Instancia de BuilderParametro del mapeador.
        /// </summary>
        public BuilderParametro BP { get; private set; }

        /// <summary>
        /// Instancia de AdoAGBD que consume el mapeador.
        /// </summary>
        /// <value></value>
        public AdoAGBD AdoAGBD { get; set; }

        /// <summary>
        /// Comando asociado al mapeador.
        /// </summary>
        /// <value></value>
        public MySqlCommand Comando { get; private set; }

        /// <summary>
        /// Nombre de la tabla asociada a la Clase
        /// </summary>
        public string Tabla { get; set; }

        /// <summary>
        /// Cadena que representa la tupa de atributos, formato (atr1, atr2, ... , atrN)
        /// </summary>
        public string TuplaAtributos { get; set; }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="ado">Instancia de AdoAGBD que consume el mapeador</param>
        public Mapeador(AdoAGBD ado)
        {
            AdoAGBD = ado;
            Comando = new MySqlCommand() { Connection = ado.Conexion };
            BP = new BuilderParametro(this);
        }

        /// <summary>
        /// Método estatico para setear un comando
        /// </summary>
        /// <param name="nombre">Nombre del Stored Procedure/Function</param>
        public void SetComandoSP(string nombre)
        {
            Comando.CommandType = CommandType.StoredProcedure;
            Comando.CommandText = nombre;
            Comando.Parameters.Clear();
        }

        /// <summary>
        /// Método para asignar a comando, en base a una acción de seteo 
        /// </summary>
        /// <param name="nombre">Nombre del SP</param>
        /// <param name="preEjecucion">Método a ejecutar previo a la ejecucion del SP</param>
        /// <param name="elemento">Elemento del mapeador</param>
        public virtual void EjecutarComandoCon(string nombre, Action<T> preEjecucion, T elemento)
        {
            SetComandoSP(nombre);
            preEjecucion(elemento);
            AdoAGBD.EjecutarComando(Comando);
        }

        /// <summary>
        /// Método para asignar a comando, en base a una acción de seteo 
        /// </summary>
        /// <param name="nombre">Nombre del SP</param>
        /// <param name="preEjecucion">Método a ejecutar previo a la ejecucion del SP</param>
        /// <param name="postEjecucion">Método a ejecutar posterior a la ejecucion del SP</param>
        /// <param name="elemento">Elemento del mapeador</param>
        public virtual void EjecutarComandoCon(string nombre, Action<T> preEjecucion, Action<T> postEjecucion, T elemento)
        {
            EjecutarComandoCon(nombre, preEjecucion, elemento);
            postEjecucion(elemento);
        }

        /// <summary>
        /// Método que agrega un parametro a la lista de parametros del comando del mapeador.
        /// </summary>
        /// <param name="parametro">Parámetro a agregar.</param>
        public void AgregarParametro(MySqlParameter parametro) => Comando.Parameters.Add(parametro);

        /// <summary>
        /// Método que en base a <c>T</c>, mapea la tabla a una colección.
        /// </summary>
        /// <param name="tabla">Tabla a recorrer.</param>
        /// <returns>Lista del tipo <c>T</c>.</returns>
        public virtual List<T> ColeccionDesdeTabla(DataTable tabla)
        {
            var lista = new List<T>();

            for (int i = 0; i < tabla.Rows.Count; i++)
            {
                T elemento = ObjetoDesdeFila(tabla.Rows[i]);
                lista.Add(elemento);
            }
            return lista;
        }

        /// <summary>
        /// Método para traer una tabla en base al nombre de la misma.
        /// </summary>
        /// <returns>Tabla devuelta.</returns>
        public List<T> ColeccionDesdeTabla()
        {
            Comando.Parameters.Clear();
            Comando.CommandType = CommandType.TableDirect;
            Comando.CommandText = Tabla;

            return ColeccionDesdeTabla(AdoAGBD.TablaPorComando(Comando));
        }

        /// <summary>
        /// Método para traer una colección en base a un SP.
        /// Es importante haber configurado el Comando previamente.
        /// Se usa <c>ObjetoDesdeFila</c> para la conversión.
        /// </summary>
        /// <returns>Colección de elementos de <c>T</c></returns>
        public List<T> ColeccionDesdeSP()
            => ColeccionDesdeTabla(AdoAGBD.TablaPorComando(Comando));

        /// <summary>
        /// Método para traer un elemento en base a un SP.
        /// Es importante haber configurado el Comando previamente.
        /// Se usa <c>ObjetoDesdeFila</c> para la conversión.
        /// </summary>
        /// <returns>Elemento del tipo <c>T</c></returns>
        public T ElementoDesdeSP()
            => ColeccionDesdeSP()[0];

        /// <summary>
        /// Método que en base a una fila, devuelve una instancia de <c>T</c>.
        /// </summary>
        /// <param name="fila">Fila generica</param>
        /// <returns>Elemento de <c>T</c> transformado desde la fila</returns>
        public abstract T ObjetoDesdeFila(DataRow fila);

        /// <summary>
        /// Método que genera una cadena para hacer INSERTS de la colección de <c>T</c>.
        /// </summary>
        /// <param name="lista">Lista de <c>T</c> para generar el INSERT</param>
        /// <returns>string con la query</returns>
        public string Insert(List<T> lista)
        {
            var sb = new StringBuilder($"INSERT INTO {Tabla} {TuplaAtributos} VALUES ");
            sb.Append('(')
              .Append(TuplaValor(lista[0]))
              .Append(')');
            for (int i = 1; i < lista.Count; i++)
            {
                sb.AppendLine(",")
                  .Append('(')
                  .Append(TuplaValor(lista[i]))
                  .Append(')');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Método que en base a <c>T</c>, genera una tupla de valores.
        /// </summary>
        /// <param name="t">Objeto del tipo <c>T</c> que se pretende parsear.</param>
        /// <returns>Cadena de la tupla de valores SIN los parentesis.</returns>
        public virtual string TuplaValor(T t) => "";

        /// <summary>
        /// Método para obtener un parametro del mapeador en base a su nombre.
        /// </summary>
        /// <param name="nombre">Nombre del parametro a buscar.</param>
        /// <returns>Parametro solicitado.</returns>
        public MySqlParameter GetParametro(string nombre) => Comando.Parameters[nombre];
        void IMapConParametros.AgregarParametro(MySqlParameter parametro)
            => AgregarParametro(parametro);
    }
}