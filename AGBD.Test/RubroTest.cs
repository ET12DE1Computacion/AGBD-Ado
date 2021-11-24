using et12.edu.ar.AGBD.Ado;
using Xunit;

namespace AGBD.Test
{
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

        [Fact]
        public void TraerRubros()
        {
            var rubros = Ado.ObtenerRubros();
            Assert.Contains(rubros, r => r.Id == 1 && r.Nombre == "Gaseosa");
        }
    }
}
