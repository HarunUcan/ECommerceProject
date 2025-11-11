using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.Options;
using Microsoft.Extensions.Options;

namespace ECommerceProject.PresentationLayer.Middlewares
{
    public class CheckUserMiddleware
    {
        private const string TempUserCookieName = "TempUserId";

        private readonly RequestDelegate _next;
        private readonly ICartService _cartService;
        private readonly TempUserCookieOptions _cookieOptions;
        private readonly ILogger<CheckUserMiddleware> _logger;

        public CheckUserMiddleware(
            RequestDelegate next,
            ICartService cartService,
            IOptions<TempUserCookieOptions> cookieOptions,
            ILogger<CheckUserMiddleware> logger)
        {
            _next = next;
            _cartService = cartService;
            _cookieOptions = cookieOptions.Value;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                EnsureTempUserCookie(context);
            }
            else
            {
                await RemoveTempUserCookieAsync(context);
            }

            await _next(context);
        }

        private void EnsureTempUserCookie(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey(TempUserCookieName))
            {
                return;
            }

            var tempUserId = string.Concat(Enumerable.Range(0, 5).Select(_ => Guid.NewGuid().ToString()));

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = _cookieOptions.RequireSecure,
                SameSite = SameSiteMode.Strict
            };

            try
            {
                _cartService.TInsert(new Cart
                {
                    TempUserId = tempUserId,
                    UpdatedDate = DateTime.UtcNow
                });

                context.Response.Cookies.Append(TempUserCookieName, tempUserId, cookieOptions);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create temporary cart for anonymous user.");
            }
        }

        private Task RemoveTempUserCookieAsync(HttpContext context)
        {
            if (!context.Request.Cookies.TryGetValue(TempUserCookieName, out var tempUserId) || string.IsNullOrWhiteSpace(tempUserId))
            {
                return Task.CompletedTask;
            }

            context.Response.Cookies.Delete(TempUserCookieName);

            try
            {
                _cartService.TDeleteByTempUserId(tempUserId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to delete temporary cart for authenticated user.");
            }

            return Task.CompletedTask;
        }
    }
}
