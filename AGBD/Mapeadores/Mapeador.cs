using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using et12.edu.ar.AGBD.Ado;

namespace et12.edu.ar.AGBD.Mapeadores;

/// <summary>
/// Clase abstracta y genérica para facilitar las configuraciones del mapeo de clases
/// </summary>
/// <typeparam name="T">El elemento sobre el que va a trabajar el Mapeador</typeparam>
public abstract class Mapeador<T> : IMapConParametros where T : class
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
    public string TuplaAtributos { get; set; } = null!;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    /// <param name="ado">Instancia de AdoAGBD que consume el mapeador</param>
    public Mapeador(AdoAGBD ado)
    {
        AdoAGBD = ado;
        Comando = new MySqlCommand() { Connection = ado.Conexion };
        BP = new BuilderParametro(this);
        Tabla = "";
    }

    /// <summary>
    /// Método estatico para setear un comando.
    /// Inicializa la lista de parámetros.
    /// </summary>
    /// <param name="nombre">Nombre del Stored Procedure/Function</param>
    public void SetComandoSP(string nombre)
    {
        Comando.CommandType = CommandType.StoredProcedure;
        Comando.CommandText = nombre;
        Comando.Parameters.Clear();
    }

    /// <summary>
    /// Método para asignar a comando, en base a una acción de seteo. 
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
    /// Método que agrega un parámetro a la lista de parámetros del comando del mapeador.
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
    /// Método para traer una tabla asincronicamente en base al nombre de la misma.
    /// </summary>
    /// <returns>Tarea con la tabla devuelta.</returns>
    public async Task<List<T>> ColeccionDesdeTablaAsync()
    {
        Comando.Parameters.Clear();
        Comando.CommandType = CommandType.TableDirect;
        Comando.CommandText = Tabla;

        return ColeccionDesdeTabla(await AdoAGBD.TablaPorComandoAsync(Comando));
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
    /// Método para traer una colección asincronicamente en base a un SP.
    /// Es importante haber configurado el Comando previamente.
    /// Se usa <c>ObjetoDesdeFila</c> para la conversión.
    /// </summary>
    /// <returns>Colección de elementos de <c>T</c></returns>
    public async Task<List<T>> ColeccionDesdeSPAsync()
        => ColeccionDesdeTabla(await AdoAGBD.TablaPorComandoAsync(Comando));

    /// <summary>
    /// Método para traer un elemento en base a un SP.
    /// Es importante haber configurado el Comando previamente.
    /// Se usa <c>ObjetoDesdeFila</c> para la conversión.
    /// </summary>
    /// <returns>Elemento del tipo <c>T</c></returns>
    public T ElementoDesdeSP()
        => ColeccionDesdeSP()[0];

    /// <summary>
    /// Método para traer un elemento en base a un SP.
    /// Es importante haber configurado el Comando previamente.
    /// Se usa <c>ObjetoDesdeFila</c> para la conversión.
    /// </summary>
    /// <returns>Elemento del tipo <c>T</c></returns>
    public async Task<T> ElementoDesdeSPAsync()
        => (await ColeccionDesdeSPAsync())[0];

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
    /// Método para obtener un parámetro del mapeador en base a su nombre.
    /// </summary>
    /// <param name="nombre">Nombre del parámetro a buscar.</param>
    /// <returns>Parámetro solicitado.</returns>
    public MySqlParameter GetParametro(string nombre) => Comando.Parameters[nombre];
    void IMapConParametros.AgregarParametro(MySqlParameter parametro)
        => AgregarParametro(parametro);

    /// <summary>
    /// Método para obtener filas filtradas por igualdad en base al diccionario que recibe
    /// </summary>
    /// <param name="diccionario">Diccionario con nombre de atributo-valor</param>
    /// <param name="tabla">Nombre de la tabla, si se omita, se usa el nombre por defecto de la tabla del Mapeador</param>
    /// <returns>DataTable asociada a la consulta</returns>
    public DataTable FilasFiltradasRAW(Dictionary<string, object> diccionario, string? tabla = null)
    {
        PrepararComandoFilasFiltradas(diccionario, tabla);
        return AdoAGBD.TablaPorComando(Comando);
    }

    /// <summary>
    /// Método para obtener filas filtradas por igualdad en base a un atributo y valor
    /// </summary>
    /// <param name="atributo">Nombre del atributo a filtrar</param>
    /// <param name="valor">Valor para filtrar</param>
    /// <param name="tabla">Nombre de la tabla, si se omita, se usa el nombre por defecto de la tabla del Mapeador</param>
    /// <returns>DataTable asociada a la consulta</returns>
    public DataTable FilasFiltradasRAW(string atributo, object valor, string? tabla = null)
        => FilasFiltradasRAW(DiccionarioPara(atributo, valor), tabla);

    /// <summary>
    /// Método para obtener filas filtradas por igualdad en base al diccionario que recibe
    /// </summary>
    /// <param name="diccionario">Diccionario con nombre de atributo-valor</param>
    /// <returns>Colección instanciada de <c>T</c> en base a <c>ObjetoDesdeFila</c></returns>
    public List<T> FilasFiltradas(Dictionary<string, object> diccionario)
        => ColeccionDesdeTabla(FilasFiltradasRAW(diccionario));

    /// <summary>
    /// Método para obtener filas filtradas por igualdad en base a un atributo y valor.
    /// </summary>
    /// <param name="atributo">Nombre del atributo a filtrar</param>
    /// <param name="valor">Valor para filtrar</param>
    /// <returns>Colección instanciada de <c>T</c> en base a <c>ObjetoDesdeFila</c></returns>
    public List<T> FilasFiltradas(string atributo, object valor)
        => ColeccionDesdeTabla(FilasFiltradasRAW(atributo, valor));

    /// <summary>
    /// Método para obtener una fila filtrada por igualdad en base a un atributo y valor.
    /// </summary>
    /// <param name="atributo">Nombre del atributo a filtrar</param>
    /// <param name="valor">Valor para filtrar</param>
    /// <returns>Objecto del tipo <c>T</c>. Tambien puede ser <c>NULL</c>.</returns>
    public T? FiltrarPorPK(string atributo, object valor)
        => FiltrarPorPK(DiccionarioPara(atributo, valor));

    /// <summary>
    /// Método para obtener una fila filtrada por igualdad en base al diccionario que recibe.
    /// </summary>
    /// <param name="diccionario">Diccionario con nombre de atributo-valor</param>
    /// <returns>Objecto del tipo <c>T</c>. Tambien puede ser <c>NULL</c>.</returns>
    public T? FiltrarPorPK(Dictionary<string, object> diccionario)
    {
        var coleccion = FilasFiltradas(diccionario);
        return coleccion.Count == 0 ? null : coleccion[0];
    }

    private Dictionary<string, object> DiccionarioPara(string atributo, object valor)
    {
        var diccionario = new Dictionary<string, object>();
        diccionario.Add(atributo, valor);
        return diccionario;
    }

    private void PrepararComandoFilasFiltradas(Dictionary<string, object> diccionario, string? tabla = null)
    {
        if (diccionario.Count == 0)
            throw new ArgumentException("El diccionario debe tener al menos 1 elemento");

        var nombreTabla = tabla ?? Tabla;
        var queryBuilder = new StringBuilder("SELECT * FROM\t").Append(nombreTabla);
        queryBuilder.AppendLine().Append("WHERE\t");
        var primero = true;
        foreach (var entrada in diccionario)
        {
            if (!primero)
                queryBuilder.AppendLine().Append("AND\t");
            else
                primero = false;
            queryBuilder.Append(entrada.Key).Append(" = ").Append(entrada.Value);
        }

        Comando.Parameters.Clear();
        Comando.CommandType = CommandType.Text;
        Comando.CommandText = queryBuilder.ToString();
    }

    /// <summary>
    /// Método para que en base a una Tabla Relacionante, poder obtener filas en base al actual Mapeador
    /// </summary>
    /// <param name="tablaRelacionante">Tabla Relacionante ya instanciada</param>
    /// <param name="FKRelacionante">Nombre del atributo FK hacia la Tabla de nuestro actual <c>Mapeador</c></param>
    /// <param name="PKdeT">Nombre de la PK de nuestro actual <c>Mapeador</c></param>
    /// <returns>Lista del tipo <c>T</c>.</returns>
    public List<T> ObtenerDesdeNN (DataTable tablaRelacionante, string FKRelacionante, string PKdeT)
    {
        if (!tablaRelacionante.Columns.Contains(FKRelacionante))
            throw new ArgumentException($"La FK {FKRelacionante} no existe en la tabla {tablaRelacionante.TableName}");
        var lista = new List<T>();
        for (int i = 0; i < tablaRelacionante.Rows.Count; i++)
        {
            var valorPK = tablaRelacionante.Rows[i][FKRelacionante];
            
            //Como existe la FK en la Relacionante, por Integridad Referencial existe en FiltrarPorPK
            lista.Add(FiltrarPorPK(PKdeT, valorPK)!);
        }
        return lista;
    }
}