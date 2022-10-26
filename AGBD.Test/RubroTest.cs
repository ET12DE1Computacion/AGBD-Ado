using et12.edu.ar.AGBD.Ado;

namespace AGBD.Test;
public class RubroTest
{
    public AdoTest Ado { get; set; }
    public RubroTest()
    {
        var adoAGBD = FactoryAdoAGBD.GetAdoMySQL("appSettings.json", "test");
        Ado = new AdoTest(adoAGBD);
    }

    [Fact]
    public void AltaRubro()
    {
        var rubro = new Rubro("Limpieza");
        Ado.AltaRubro(rubro);
        Assert.Equal(3, rubro.Id);
    }

    [Theory]
    [InlineData(1, "Gaseosa")]
    [InlineData(2, "Lacteo")]
    public void TraerRubros(byte id, string nombre)
    {
        var rubros = Ado.ObtenerRubros();
        Assert.Contains(rubros, r => r.Id == id && r.Nombre == nombre);
    }
}
