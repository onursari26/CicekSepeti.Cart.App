using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace CicekSepeti.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult ResponseResult<T>(ResponseInfo<T> result)
        {
            return result.HttpStatusCode switch
            {
                HttpStatusCode.OK => Ok(result),
                HttpStatusCode.Created => Created(new Uri(string.IsNullOrWhiteSpace(result.ReturnUrl) ? $"{Request.Path}/{result.ReturnUrl}" : result.ReturnUrl, UriKind.Relative), result),
                HttpStatusCode.Accepted => Accepted(result),
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.BadRequest => BadRequest(result),
                HttpStatusCode.Unauthorized => Unauthorized(),
                HttpStatusCode.Forbidden => Forbid(),
                HttpStatusCode.NotFound => NotFound(result),
                HttpStatusCode.MethodNotAllowed => NotFound(result),
                HttpStatusCode.Conflict => Conflict(result),
                HttpStatusCode.InternalServerError => BadRequest(),
                _ => Ok(result),
            };
        }
    }
}
