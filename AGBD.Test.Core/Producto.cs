namespace AGBD.Test.Core;

public class Producto
{
    public short Id { get; set; }
    public Rubro Rubro { get; set; }
    public string Nombre { get; set; }
    public decimal PrecioUnitario { get; set; }
    public ushort Cantidad { get; set; }

    public override string ToString()
        => $"{Nombre} - {Rubro.Nombre} - Cantidad: {Cantidad} - ${PrecioUnitario:0.00}c/u";
}