using et12.edu.ar.AGBD.Ado;

namespace AGBD.Test;

public class ProductoTest
{
    public AdoTest Ado { get; set; }
    public ProductoTest()
    {
        var adoAGBD = FactoryAdoAGBD.GetAdoMySQL("appSettings.json", "test");
        Ado = new AdoTest(adoAGBD);
    }

    [Fact]
    public void TraerProductos()
    {
        var productos = Ado.ObtenerProductos();
        Assert.Equal(2, productos.Count);
        Assert.Contains(productos, p => p.Id == 10 && p.Nombre == "Manaos Cola 2.25L.");
    }

    [Fact]
    public void AltaProducto()
    {
        var lacteo = new Rubro("Lacteo") { Id = 2 };
        var leche = new Producto()
        {
            Nombre = "Armonia 1L.",
            PrecioUnitario = 70,
            Cantidad = 49,
            Rubro = lacteo
        };

        Ado.AltaProducto(leche);
        var productos = Ado.ObtenerProductos(lacteo);
        Assert.Contains(productos, p => p.Nombre == "Armonia 1L." && p.Cantidad == 49);

    }
}
