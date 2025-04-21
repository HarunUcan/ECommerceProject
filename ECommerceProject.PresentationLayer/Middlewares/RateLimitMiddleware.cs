namespace ECommerceProject.PresentationLayer.Middlewares
{
    public class RateLimitMiddleware
    {
        private static Dictionary<string, (DateTime lastAttempt, int count, DateTime? banUntil)> _requests = new();
        private readonly RequestDelegate _next;
        private readonly int _limit = 40;
        private readonly TimeSpan _window = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _banDuration = TimeSpan.FromMinutes(1);

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Eğer kullanıcı admin ise sınır yok
            if (context.User.Identity?.IsAuthenticated == true &&
                context.User.IsInRole("Admin"))
            {
                await _next(context);
                return;
            }

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (_requests.TryGetValue(ip, out var entry))
            {
                if (entry.banUntil != null && entry.banUntil > DateTime.UtcNow)
                {
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsync("You are temporarily banned.");
                    return;
                }

                if (entry.lastAttempt.Add(_window) > DateTime.UtcNow)
                {
                    entry.count++;
                    if (entry.count > _limit)
                    {
                        entry.banUntil = DateTime.UtcNow.Add(_banDuration);
                        _requests[ip] = entry;
                        context.Response.StatusCode = 429;
                        await context.Response.WriteAsync("Too many requests. You are now banned.");
                        return;
                    }
                }
                else
                {
                    entry.count = 1;
                    entry.lastAttempt = DateTime.UtcNow;
                    entry.banUntil = null;
                }

                _requests[ip] = entry;
            }
            else
            {
                _requests[ip] = (DateTime.UtcNow, 1, null);
            }

            await _next(context);
        }
    }

}
