using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AGBD.Test.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AGBD.Test.MVC.Controllers;

public class RubroController : Controller
{
    private readonly Servicio _servicio;

    public RubroController(Servicio servicio) => _servicio = servicio;

    public IActionResult Index()
    {
        var rubros = _servicio.ObtenerRubros();
        return View("Listado", rubros);
    }
}
