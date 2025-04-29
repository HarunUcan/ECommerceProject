using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    public class PaymentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICartService _cartService;
        private readonly ICategoryService _categoryService;

        public PaymentController(ICartService cartService, ICategoryService categoryService, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _cartService = cartService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            var homeViewModel = new HomeViewModel
            {
                Categories = _categoryService.TGetList(),
                Cart = cart
            };
            return View(homeViewModel);
        }

        public async Task<IActionResult> IyzicoExamplePayment()
        {
            Options options = new Options()
            {
                ApiKey = "sandbox-F6uw4W12sdhw4AxjzYVYITZbc43z1cwi",
                SecretKey = "sandbox-2E5YPn8Qt8sI2DhL1WzBup4CF5EoKJDS",
                BaseUrl = "https://sandbox-api.iyzipay.com"
            };

            CreatePaymentRequest request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = "123456789",
                Price = 1498.99.ToString(),
                PaidPrice = (1498.99 * 1.1).ToString(),
                Currency = Currency.TRY.ToString(),
                Installment = 3,
                BasketId = "BASKETID",
                PaymentChannel = PaymentChannel.WEB.ToString(),
                PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                CallbackUrl = "https://localhost:44321/User/Payment/IyzicoPaymentCallback",
            };

            PaymentCard card = new PaymentCard
            {
                CardHolderName = "John Doe",
                CardNumber = "5528790000000008",
                ExpireMonth = "12",
                ExpireYear = "2030",
                Cvc = "123",
                RegisterCard = 0
            };
            request.PaymentCard = card;

            Buyer buyer = new Buyer
            {
                Id = "BY789",
                Name = "John",
                Surname = "Doe",
                GsmNumber = "+905350000000",
                Email = "example@example.com",
                IdentityNumber = "11111111111",
                LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                RegistrationAddress = "Address",
                Ip = "127.0.0.1",
                City = "Istanbul",
                Country = "Turkey",
                ZipCode = "34732"
            };
            request.Buyer = buyer;

            Address shippingAddress = new Address
            {
                ContactName = "Jane Doe",
                City = "Istanbul",
                Country = "Turkey",
                Description = "Shipping Address",
                ZipCode = "34732",
            };
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address
            {
                ContactName = "Jane Doe",
                City = "Istanbul",
                Country = "Turkey",
                Description = "Billing Address",
                ZipCode = "34732",
            };
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>
            {
                new BasketItem
                {
                    Id = "BI101",
                    Name = "Product 1",
                    Category1 = "Category 1",
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Price = "799.99"
                },
                new BasketItem
                {
                    Id = "BI102",
                    Name = "Product 2",
                    Category1 = "Category 2",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = "699.00"
                }
            };
            request.BasketItems = basketItems;

            // Initialize the payment client
            ThreedsInitialize threedsInitialize = await ThreedsInitialize.Create(request, options);

            if (threedsInitialize.Status == "success")
            {
                // Redirect to the 3D Secure page
                //return Redirect(threedsInitialize.HtmlContent);
                return Content(threedsInitialize.HtmlContent, "text/html");
            }
            else
            {
                // Handle error
                //ModelState.AddModelError("", "Payment initialization failed.");
                //return View("Index");
                return Content("Payment initialization failed.", "text/html");
            }
        }

        public IActionResult IyzicoPaymentCallback(IFormCollection iFormCollection)
        {
            // 3ds dan dönen callback verilerini işlenir
            if (iFormCollection["status"] == "success")
            {
                return Content("Payment successful!", "text/html");
            }
            else
            {
                return Content("Payment failed!", "text/html");
            }

        }

        [HttpGet]
        public async Task<IActionResult> IyzicoInstallmentCheck(string price, string binNumber = "554960")
        {
            Options options = new Options
            {
                ApiKey = "sandbox-F6uw4W12sdhw4AxjzYVYITZbc43z1cwi",
                SecretKey = "sandbox-2E5YPn8Qt8sI2DhL1WzBup4CF5EoKJDS",
                BaseUrl = "https://sandbox-api.iyzipay.com"
            };

            RetrieveInstallmentInfoRequest request = new RetrieveInstallmentInfoRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = "123456789";
            request.BinNumber = binNumber.ToString();
            request.Price = price.ToString();

            InstallmentInfo installmentInfo = await InstallmentInfo.Retrieve(request, options);

            if (installmentInfo.Status == "success")
            {
                // İşlem başarılı
                return Json(installmentInfo);
            }
            else
            {
                // İşlem başarısız
                return Json(new { error = installmentInfo.ErrorMessage });
            }
        }
            
    }
}
