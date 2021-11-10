using System;
using Xunit;
using et12.edu.ar.AGBD.Ado;
using System.Linq;

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
            var rubro = new Rubro("Lacteo");
            Ado.AltaRubro(rubro);
            Assert.Equal(2, rubro.Id);
        }

        [Fact]
        public void TraerRubros()
        {
            var rubros = Ado.ObtenerRubros();
            Assert.Contains(rubros, r => r.Id == 1 && r.Nombre == "Gaseosa");
        }


    }
}
