using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.DtoLayer.Dtos.PaymentDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers;

[Area("User")]
public class PaymentController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICartService _cartService;
    private readonly ICategoryService _categoryService;
    private readonly IAdressService _adressService;
    private readonly IProductService _productService;
    private readonly ISaleService _saleService;
    private readonly Options _iyzicoOptions;

    public PaymentController(ICartService cartService, ICategoryService categoryService, UserManager<AppUser> userManager, IAdressService adressService, IProductService productService, ISaleService saleService, IOptions<Options> iyzicoOptions)
    {
        _userManager = userManager;
        _cartService = cartService;
        _categoryService = categoryService;
        _adressService = adressService;
        _productService = productService;
        _saleService = saleService;
        _iyzicoOptions = iyzicoOptions.Value;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user?.Id ?? 0);
        List<Adress>? userAdresses = user != null ? _adressService.TGetAdressesByUserId(user.Id) : null;

        var paymentViewModel = new PaymentViewModel
        {
            Categories = _categoryService.TGetList(),
            Cart = cart,
            Adresses = userAdresses,
        };
        return View(paymentViewModel);
    }

    [HttpGet]
    public IActionResult PaymentStatus(bool isSuccess, string? price, string? paymentMethod, string? transactionId)
    {
        ViewData["IsSuccess"] = isSuccess;
        ViewData["AmountPaid"] = price ?? "0.00";
        ViewData["PaymentMethod"] = paymentMethod ?? "Credit Card";
        ViewData["TransactionId"] = transactionId ?? "N/A";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> IyzicoPaymentInitialize(string holderName, string cardNumber, string expiryDate, string cvc, int selectedAdressId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            CartDetailDto? cart = user != null
                ? await _cartService.TGetCartDetailsAsync(null, user.Id)
                : await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("PaymentStatus", new { isSuccess = false });
            }

            Adress? userAdress = null;
            if (user != null)
            {
                var userAdresses = _adressService.TGetAdressesByUserId(user.Id);
                userAdress = userAdresses.FirstOrDefault(x => x.AdressId == selectedAdressId);
                if (userAdress == null)
                {
                    return RedirectToAction("PaymentStatus", new { isSuccess = false });
                }
            }

            CartPricingResult pricing = CartPricingCalculator.CalculateTotals(cart);

            List<BasketItem> basketItems = cart.Items.Select(ci => new BasketItem
            {
                Id = ci.ProductId.ToString(),
                Name = ci.ProductName,
                Category1 = ci.CategoryName,
                ItemType = BasketItemType.PHYSICAL.ToString(),
                Price = GetDiscountedUnitPrice(ci).ToString(CultureInfo.InvariantCulture)
            }).ToList();

            PaymentCard card = new PaymentCard
            {
                CardHolderName = holderName,
                CardNumber = cardNumber,
                ExpireMonth = expiryDate.Split('/')[0],
                ExpireYear = expiryDate.Split('/')[1],
                Cvc = cvc,
                RegisterCard = 0
            };

            (string firstName, string lastName) = ResolveBuyerName(user, holderName);

            Sale sale = new Sale
            {
                SaleItems = cart.Items.Select(ci => new SaleItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Size = ci.Size,
                    SoldUnitPrice = GetDiscountedUnitPrice(ci)
                }).ToList(),
                SaleDate = DateTime.Now,
                CustomerName = firstName,
                CustomerSurname = lastName,
                CustomerEmail = user?.Email,
                CustomerPhone = user?.PhoneNumber,
                AdressId = userAdress?.AdressId,
                SaleStatus = SaleStatus.NotApproved,
                AppUserId = user?.Id,
                TempCartId = user == null ? Request.Cookies["TempUserId"] : null,
                TotalPrice = pricing.FinalTotal,
                InstallmentCount = 1,
                PaymentMethod = PaymentMethod.CreditCard
            };

            _saleService.TInsert(sale);

            Buyer buyer = BuildBuyer(user, holderName, firstName, lastName, userAdress, sale);

            Address shippingAddress = BuildAddress(firstName, lastName, userAdress);
            Address billingAddress = BuildAddress(firstName, lastName, userAdress);

            if (string.IsNullOrWhiteSpace(_iyzicoOptions.ApiKey) || string.IsNullOrWhiteSpace(_iyzicoOptions.SecretKey) || string.IsNullOrWhiteSpace(_iyzicoOptions.BaseUrl))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Payment provider configuration is missing.");
            }

            CreatePaymentRequest request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = sale.SaleId.ToString(),
                Price = basketItems.Sum(item => Convert.ToDecimal(item.Price, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture),
                PaidPrice = pricing.FinalTotal.ToString(CultureInfo.InvariantCulture),
                Currency = Currency.TRY.ToString(),
                Installment = 1,
                BasketId = user != null ? cart.CartId.ToString() : Request.Cookies["TempUserId"],
                PaymentChannel = PaymentChannel.WEB.ToString(),
                PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                CallbackUrl = "https://localhost:44321/User/Payment/IyzicoPaymentCallback",
                BasketItems = basketItems,
                PaymentCard = card,
                Buyer = buyer,
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress
            };

            ThreedsInitialize threedsInitialize = await ThreedsInitialize.Create(request, _iyzicoOptions);

            if (threedsInitialize.Status == "success")
            {
                if (Request.Cookies["TempUserId"] != null)
                {
                    await CleanTheCartAsync(Request.Cookies["TempUserId"], 0, cart);
                }
                else if (user != null)
                {
                    await CleanTheCartAsync(null, user.Id, cart);
                }

                return Content(threedsInitialize.HtmlContent, "text/html");
            }

            sale.SaleStatus = SaleStatus.Unsuccessful;
            _saleService.TUpdate(sale);
            return RedirectToAction("PaymentStatus", new { isSuccess = false });
        }
        catch
        {
            return RedirectToAction("PaymentStatus", new { isSuccess = false });
        }
    }

    public IActionResult IyzicoPaymentCallback(IFormCollection iFormCollection)
    {
        if (iFormCollection["status"] == "success")
        {
            var saleId = iFormCollection["conversationId"];
            var sale = _saleService.TGetById(Convert.ToInt32(saleId));
            sale.SaleStatus = SaleStatus.PaymentApproved;
            _saleService.TUpdate(sale);

            var totalPrice = sale.TotalPrice.ToString(CultureInfo.InvariantCulture);
            var method = sale.PaymentMethod.ToString();

            return RedirectToAction("PaymentStatus", new
            {
                isSuccess = true,
                price = totalPrice,
                paymentMethod = method,
                transactionId = saleId
            });
        }

        return RedirectToAction("PaymentStatus", new { isSuccess = false });
    }

    [HttpGet]
    public async Task<IActionResult> IyzicoInstallmentCheck(string price, string binNumber)
    {
        var user = await _userManager.GetUserAsync(User);

        CartDetailDto? cart = user != null
            ? await _cartService.TGetCartDetailsAsync(null, user.Id)
            : await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

        CartPricingResult pricing = CartPricingCalculator.CalculateTotals(cart);

        RetrieveInstallmentInfoRequest request = new RetrieveInstallmentInfoRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = "123456789",
            BinNumber = binNumber,
            Price = pricing.FinalTotal.ToString(CultureInfo.InvariantCulture)
        };

        if (string.IsNullOrWhiteSpace(_iyzicoOptions.ApiKey) || string.IsNullOrWhiteSpace(_iyzicoOptions.SecretKey) || string.IsNullOrWhiteSpace(_iyzicoOptions.BaseUrl))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Payment provider configuration is missing.");
        }

        InstallmentInfo installmentInfo = await InstallmentInfo.Retrieve(request, _iyzicoOptions);

        if (installmentInfo.Status == "success")
        {
            return Json(installmentInfo);
        }

        return Json(new { error = installmentInfo.ErrorMessage });
    }

    private async Task CleanTheCartAsync(string? tempUserId, int userId, CartDetailDto cart)
    {
        foreach (var item in cart.Items)
        {
            var product = _productService.TGetById(item.ProductId);
            if (product != null)
            {
                product.Stock += item.Quantity;
                _productService.TUpdate(product);
            }

            await _cartService.TDeleteCartItemAsync(tempUserId, userId, item.ProductId, item.Size);
        }
    }

    private static (string FirstName, string LastName) ResolveBuyerName(AppUser? user, string holderName)
    {
        if (user != null)
        {
            return (user.Name ?? string.Empty, user.Surname ?? string.Empty);
        }

        var nameParts = holderName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (nameParts.Length == 0)
        {
            return ("", "");
        }

        var lastName = nameParts[^1];
        var firstName = string.Join(' ', nameParts[..^1]);
        return (firstName, lastName);
    }

    private static decimal GetDiscountedUnitPrice(CartItemDetailDto item)
    {
        decimal unitPrice = item.Price;
        if (item.DiscountRate.HasValue)
        {
            unitPrice -= unitPrice * item.DiscountRate.Value / 100m;
        }
        else if (item.DiscountAmount.HasValue)
        {
            unitPrice -= item.DiscountAmount.Value;
        }

        return unitPrice < 0 ? 0 : unitPrice;
    }

    private Buyer BuildBuyer(AppUser? user, string holderName, string firstName, string lastName, Adress? userAdress, Sale sale)
    {
        return new Buyer
        {
            Id = user != null ? user.Id.ToString() : sale.SaleId.ToString(),
            Name = string.IsNullOrWhiteSpace(firstName) ? holderName : firstName,
            Surname = string.IsNullOrWhiteSpace(lastName) ? holderName : lastName,
            GsmNumber = user?.PhoneNumber ?? string.Empty,
            Email = user?.Email ?? "tempuser@example.com",
            IdentityNumber = "11111111111",
            LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            RegistrationAddress = userAdress?.AdressLine ?? "Address",
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
            City = userAdress?.City ?? "İstanbul",
            Country = "Türkiye",
            ZipCode = "34732"
        };
    }

    private static Address BuildAddress(string firstName, string lastName, Adress? userAdress)
    {
        return new Address
        {
            ContactName = string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName)
                ? "Müşteri"
                : $"{firstName} {lastName}".Trim(),
            City = userAdress?.City ?? "İstanbul",
            Country = "Türkiye",
            Description = userAdress?.AdressLine ?? "Address",
            ZipCode = "34732",
        };
    }
}
