using E_Commerce.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIBaseController : ControllerBase
    {

        public static ActionResult<T>ToActionResult<T>(Result<T> result)
        {

            if (result.IsSuccess)
            {

                return new OkObjectResult(result.data);

            }
            return ToProplem(result.Errors);

        }

        public static ActionResult ToActionResult(Result result)
        {

            if (result.IsSuccess)
            {

                return new OkResult();

            }
            return ToProplem(result.Errors);

        }



        private static ObjectResult ToProplem(IReadOnlyList<Error> errors)
        {

            var first = errors[0];

            var status = first.Type switch
           
            {

                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError

            };

            var proplem = new ProblemDetails
            {

                Status = status,
                Title = first.code,
                Detail = first.description,
                Extensions = { ["errors"] = errors }

            };

            return new ObjectResult(proplem) { StatusCode = status };



        }

        protected string GetEmailFromToken()
            => User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedAccessException("No Email Clain Found");


    }
}
