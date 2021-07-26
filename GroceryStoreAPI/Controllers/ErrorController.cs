using GroceryStoreDataRepo;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Controllers
{
    /// <summary>
    /// handles and logs all errors that occur in the api
    /// </summary>
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("/error")]
        [HttpGet]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            _logger.LogError(context.Error, "api exception");

            if (context.Error is ClientException)
            {
                return Problem(
                    statusCode: 400,
                    detail: string.Empty,
                    title: context.Error.Message);
            }
            else
                return Problem();
        }
    }
}
