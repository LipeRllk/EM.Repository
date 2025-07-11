using AspNetCoreGeneratedDocument;
using EM.Domain.Context;
using EM.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EM.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult CidadeListAAA()
        {

            return View("~/Views/Cidadeteste/CidadeListAAA.cshtml");
        }
    }
}


