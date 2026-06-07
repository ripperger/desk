using Desk.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Desk.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            ViewBag.ErrorMessage = "Sajnos ";
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage += "a keresett oldal nem található.";
                    break;
                case 500:
                    ViewBag.ErrorMessage += "szerver oldali hiba történt.";
                    break;
                default:
                    ViewBag.ErrorMessage += "váratlan hiba történt.";
                    break;
            }

            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Error")]
        public IActionResult Error()
        {
            ViewBag.ErrorMessage = "Sajnos váratlan hiba történt.";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
