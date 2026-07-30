using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace E_Commerce.API.Attributes
{
    public class RedisCasheAttribute : ActionFilterAttribute
    {
        private readonly int _durationSec;

        public RedisCasheAttribute(int durationSec = 90)
        {
            _durationSec = durationSec;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the cache service from Container [not injection direct into constructor]
            // if data is found in cache then return it and skip the action execution
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICasheService>();

            var cacheKey = CreateCasheKey(context.HttpContext.Request);

            var cashed = await cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cashed))
            {
                context.Result = new ContentResult
                {
                    Content = cashed,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }

            var executed = await next.Invoke();

            if (executed.Result is OkObjectResult { Value: not null } ok)
                await cacheService.SetAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(_durationSec));
        }

        private string CreateCasheKey(HttpRequest request)
        {
            var key = new StringBuilder();

            key.Append(request.Path).Append('?');

            foreach (var (k, v) in request.Query.OrderBy(q => q.Key))
                key.Append(k).Append("=").Append(v).Append("&");

            return key.ToString();
        }
    }
}
