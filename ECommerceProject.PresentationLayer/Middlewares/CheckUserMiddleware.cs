using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.Middlewares
{
    public class CheckUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Kullanıcı giriş yapmış mı kontrol et
            if (context.User.Identity?.IsAuthenticated == false) // Kullanıcı giriş yapmamışsa
            {
                // TempUserId çerezi var mı kontrol et
                if (!context.Request.Cookies.ContainsKey("TempUserId"))
                {
                    var tempUserId = Guid.NewGuid().ToString();
                    tempUserId += Guid.NewGuid().ToString();
                    tempUserId += Guid.NewGuid().ToString();
                    tempUserId += Guid.NewGuid().ToString();
                    tempUserId += Guid.NewGuid().ToString();

                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7), // 7 gün boyunca geçerli olacak
                        HttpOnly = true, // Güvenlik için JavaScript erişimini engelle
                        Secure = true, // HTTPS zorunlu (HTTP kullanıyorsan false yap)
                        SameSite = SameSiteMode.Strict // Çerez paylaşımını sınırla
                    };

                    Context dbContext = new Context();
                    dbContext.Carts.Add(new Cart 
                    { 
                        TempUserId = tempUserId,
                        UpdatedDate = DateTime.Now
                    });
                    dbContext.SaveChanges();

                    // Çerezi oluştur
                    context.Response.Cookies.Append("TempUserId", tempUserId, cookieOptions);
                }
            }

            // Iyzico ödemesi yapıldıktan sonra kullanıcı giriş yapmış olduğu halde TempUserId çerezi oluşturuluyor bu yüzden burada siliyoruz 
            // Şu anlık problem oluşturmuyor gibi görünüyor ancak sepet taşıma işlemlerinde çalışma sırasına bağlı olarak sorun çıkarabilir
            else
            {
                if (context.Request.Cookies.ContainsKey("TempUserId"))
                {
                    // Kullanıcı giriş yapmışsa ve TempUserId çerezi varsa, çerezi sil
                    context.Response.Cookies.Delete("TempUserId");

                    // Kullanıcıya ait sepeti güncelle
                    var tempUserId = context.Request.Cookies["TempUserId"];
                    Context dbContext = new Context();
                    var cart = dbContext.Carts.FirstOrDefault(c => c.TempUserId == tempUserId);
                    if (cart != null)
                    {
                        dbContext.Carts.Remove(cart);
                        dbContext.SaveChanges();
                    }
                }
            }

            await _next(context);
        }
    }

}
