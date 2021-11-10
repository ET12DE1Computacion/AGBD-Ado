using System;
using System.Collections.Generic;
using et12.edu.ar.AGBD.Ado;

namespace AGBD.Test
{
    public class AdoTest
    {
        public AdoAGBD Ado { get; set; }
        public MapRubro MapRubro { get; set; }
        public AdoTest(AdoAGBD ado)
        {
            Ado = ado;
            MapRubro = new MapRubro(Ado);
        }
        public void AltaRubro(Rubro rubro) => MapRubro.AltaRubro(rubro);
        public List<Rubro> ObtenerRubros() => MapRubro.ObtenerRubros();
    }
}
