using Microsoft.AspNetCore.Mvc;
using WebApiProject.Web.Models.Responses;

namespace WebApiProject.Web.Extensions
{
    public static class ResponseExtensions
    {
        public static IActionResult AsActionResult(this Response response)
        {
            return response.Status switch
            {
                ResponseStatus.Ok => new OkObjectResult(response),
                ResponseStatus.Error => new BadRequestObjectResult(response),
                ResponseStatus.NotFound => new NotFoundObjectResult(response),
                _ => new StatusCodeResult(500)
            };
        }
    }
}