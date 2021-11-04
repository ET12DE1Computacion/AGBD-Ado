using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace et12.edu.ar.AGBD.Mapeadores
{
    /// <summary>
    /// Clase abstracta y genérica para facilitar las configuraciones del mapeo de clases
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Mapeador<T>
    {
        /// <summary>
        /// Instancia de BuilderParametro del mapeador.
        /// </summary>
        public BuilderParametro BP { get; private set; } = new BuilderParametro();
        
        /// <summary>
        /// Nombre de la tabla asociada a la Clase
        /// </summary>
        public string Tabla { get; set; }

        /// <summary>
        /// Cadena que representa la tupa de atributos, formato (atr1, atr2, ... , atrN)
        /// </summary>
        public string TuplaAtributosInsert { get; set; }

        /// <summary>
        /// Método estatico para setear un comando
        /// </summary>
        /// <param name="comando">Comando al que hay que asignar</param>
        /// <param name="nombre">Nombre del Stored Procedure/Function</param>
        public static void SetComandoSP(MySqlCommand comando, string nombre)
        {
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = nombre;
            comando.Parameters.Clear();
        }

        /// <summary>
        /// Método para asignar a comando, en base a una acción de seteo 
        /// </summary>
        /// <param name="comando"></param>
        /// <param name="accionSeteo"></param>
        /// <param name="elemento"></param>
        public virtual void SetComandoPara(MySqlCommand comando, Action<T> accionSeteo, T elemento)
        {
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Clear();
            accionSeteo(elemento);
        }
        
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
        /// Método que en base a una fila, devuelve una instancia de <c>T</c>.
        /// </summary>
        /// <param name="fila">fila </param>
        /// <returns></returns>
        public abstract T ObjetoDesdeFila(DataRow fila);

        /// <summary>
        /// Método que genera una cadena para hacer INSERTS de la colección de <c>T</c>.
        /// </summary>
        /// <param name="lista">Lista de <c>T</c> para generar el INSERT</param>
        /// <returns></returns>
        public virtual string Insert(List<T> lista)
        {
            var sb = new StringBuilder($"INSERT INTO {Tabla} {TuplaAtributosInsert} VALUES ");
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
    }
}