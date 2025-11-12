using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.PaymentDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using IyzipayBasketItem = Iyzipay.Model.BasketItem;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    public class PaymentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IBasketService _basketService;
        private readonly ICategoryService _categoryService;
        private readonly IAdressService _adressService;
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;
        private readonly Iyzipay.Options _iyzicoOptions;

        public PaymentController(
            IBasketService basketService,
            ICategoryService categoryService,
            UserManager<AppUser> userManager,
            IAdressService adressService,
            IProductService productService,
            ISaleService saleService,
            IOptions<Iyzipay.Options> iyzicoOptions)
        {
            _userManager = userManager;
            _basketService = basketService;
            _categoryService = categoryService;
            _adressService = adressService;
            _productService = productService;
            _saleService = saleService;
            _iyzicoOptions = iyzicoOptions.Value;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            Basket basket = null;
            List<Adress> userAdresses;

            if (user != null)
            {
                basket = _basketService.TGetBasket(Request.Cookies["tempUserId"], user.Id);
                userAdresses = _adressService.TGetAdressesByUserId(user.Id);
            }
            else
            {
                basket = _basketService.TGetBasket(Request.Cookies["tempUserId"], 0);
                userAdresses = null;
            }

            var paymentViewModel = new PaymentViewModel
            {
                Categories = _categoryService.TGetList(),
                Basket = basket,
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
        public async Task<IActionResult> IyzicoPaymentInitialize(string holderName, string cardNumber, string expiryDate, string cvc, int selectedAdressId)//PaymentDetailDto paymentDetailDto
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                //if(user == null)
                //    return Content("Payment initialization failed.", "text/html");

                Basket? basket;
                Adress? userAdress = null;
                if (user != null)
                {
                    basket = _basketService.TGetBasket(null, user.Id);
                    var userAdresses = _adressService.TGetAdressesByUserId(user.Id);
                    if(userAdresses.FirstOrDefault(x => x.AdressId == selectedAdressId) != null)
                        userAdress = userAdresses.FirstOrDefault(x => x.AdressId == selectedAdressId);
                    else
                        return RedirectToAction("PaymentStatus", new { isSuccess = false });
                }
                else
                    basket = _basketService.TGetBasket(Request.Cookies["tempUserId"], 0);

                //var priceOfBasket = basket.BasketItems.Sum(ci => ci.Product.Price);
                decimal priceOfBasket = 0;

                // Ürünlerin fiyatlarını ve indirimlerini hesapla
                foreach (var item in basket.BasketItems)
                {
                    var product = _productService.TGetById(item.ProductId);
                    if (product != null)
                    {
                        if (product.DiscountRate != null) // product.DiscountEndDate != null && product.DiscountEndDate > DateTime.Now && 
                        {
                            priceOfBasket += (product.Price - (product.Price * (product.DiscountRate ?? 0) / 100)) * item.Quantity;
                        }
                        else if (product.DiscountAmount != null) // product.DiscountEndDate != null && product.DiscountEndDate > DateTime.Now && 
                        {
                            priceOfBasket += (product.Price - product.DiscountAmount.Value) * item.Quantity;
                        }
                        else
                        {
                            priceOfBasket += product.Price * item.Quantity;
                        }
                    }
                }

                // Sepetteki kuponları kontrol et ve varsa indirimleri uygula
                foreach (var cc in basket.BasketCoupons)
                {
                    var coupon = cc.Coupon;
                    if (coupon.DiscountPercentage != null)
                    {
                        priceOfBasket = priceOfBasket - (priceOfBasket * coupon.DiscountPercentage.Value / 100);
                    }
                    else if (coupon.DiscountAmount != null)
                    {
                        priceOfBasket -= coupon.DiscountAmount.Value;
                    }
                }

                List<IyzipayBasketItem> iyzicoBasketItems = basket.BasketItems.Select(ci => new IyzipayBasketItem
                {
                    Id = ci.Product.ProductId.ToString(),
                    Name = ci.Product.Name,
                    Category1 = _categoryService.TGetById(ci.Product.CategoryId).Name,
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = ci.Product.DiscountRate != null ?
                        (ci.Product.Price - (ci.Product.Price * (ci.Product.DiscountRate ?? 0) / 100)).ToString() :
                        ci.Product.DiscountAmount != null ?
                        (ci.Product.Price - ci.Product.DiscountAmount).ToString() :
                        ci.Product.Price.ToString()

                    //Price = ci.Product.DiscountEndDate != null && ci.Product.DiscountEndDate > DateTime.Now && ci.Product.DiscountRate != null ?
                    //    (ci.Product.Price - (ci.Product.Price * (ci.Product.DiscountRate ?? 0) / 100)).ToString() :
                    //    ci.Product.DiscountEndDate != null && ci.Product.DiscountEndDate > DateTime.Now && ci.Product.DiscountAmount != null ?
                    //    (ci.Product.Price - ci.Product.DiscountAmount).ToString() :
                    //    ci.Product.Price.ToString()
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

                string name = "";
                string surname = "";
                if (user != null)
                {
                    name = user.Name;
                    surname = user.Surname;
                }
                else
                {
                    string[] nameStr = holderName.Split(' ');
                    for (int i = 0; i < nameStr.Length; i++)
                    {
                        if (i == nameStr.Length - 1)
                            surname = nameStr[i];
                        else
                            name += nameStr[i] + " ";
                    }
                }

                Buyer buyer = new Buyer
                {
                    Id = user != null ? user.Id.ToString() : Request.Cookies["tempUserId"],
                    Name = name,
                    Surname = surname,
                    GsmNumber = user?.PhoneNumber ?? "",
                    Email = user?.Email ?? "tempuser@example.com",
                    IdentityNumber = "11111111111",
                    LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    RegistrationAddress = "Address",
                    Ip = "127.0.0.1",
                    City = userAdress.City,
                    Country = "Türkiye",
                    ZipCode = "34732"
                };

                Address shippingAddress = new Address
                {
                    ContactName = $"{name} {surname}",
                    City = userAdress.City,
                    Country = "Türkiye",
                    Description = userAdress.AdressLine,
                    ZipCode = "34732",
                };

                Address billingAddress = new Address
                {
                    ContactName = $"{name} {surname}",
                    City = userAdress.City,
                    Country = "Türkiye",
                    Description = userAdress.AdressLine,
                    ZipCode = "34732",
                };

                var basketEntityItems = basket.BasketItems.ToList();

                Sale sale = new Sale
                {
                    SaleItems = basketEntityItems.Select(ci => new SaleItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Size = ci.Size,
                        SoldUnitPrice = ci.Product.DiscountRate != null ?
                            (ci.Product.Price - (ci.Product.Price * (ci.Product.DiscountRate ?? 0) / 100)) :
                            ci.Product.DiscountAmount != null ?
                            (ci.Product.Price - ci.Product.DiscountAmount.Value) :
                            ci.Product.Price
                    }).ToList(),
                    SaleDate = DateTime.Now,
                    CustomerName = name,
                    CustomerSurname = surname,
                    CustomerEmail = user?.Email,
                    CustomerPhone = user?.PhoneNumber,
                    AdressId = userAdress.AdressId,
                    SaleStatus = SaleStatus.NotApproved,
                    AppUserId = user?.Id,
                    TempBasketId = user == null ? Request.Cookies["tempUserId"] : null,
                    TotalPrice = priceOfBasket,
                    InstallmentCount = 1, // Taksit sayısı
                    PaymentMethod = PaymentMethod.CreditCard // Ödeme yöntemi
                };
                // Veritabanına kaydetme işlemi
                _saleService.TInsert(sale);


                CreatePaymentRequest request = new CreatePaymentRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = sale.SaleId.ToString(), // bu kısımda veritabanında oluşturulan satış verisinin id si kullanılacak daha onaylanmadığı için SaleStatus = SaleStatus.NotApproved olacak 3d secure işleminden sonra onaylanacak başarısızsa SaleStatus = SaleStatus.Unsuccessful olacak
                    Price = iyzicoBasketItems.Sum(item => Convert.ToDecimal(item.Price)).ToString(),
                    PaidPrice = priceOfBasket.ToString(),
                    Currency = Currency.TRY.ToString(),
                    Installment = 1,
                    BasketId = user != null ? basket.BasketId.ToString() : Request.Cookies["tempUserId"],
                    PaymentChannel = PaymentChannel.WEB.ToString(),
                    PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                    CallbackUrl = "https://localhost:44321/User/Payment/IyzicoPaymentCallback",
                };

                request.BasketItems = iyzicoBasketItems;

                request.PaymentCard = card;

                request.Buyer = buyer;

                request.ShippingAddress = shippingAddress;

                request.BillingAddress = billingAddress;

                // Initialize the payment client
                ThreedsInitialize threedsInitialize = await ThreedsInitialize.Create(request, _iyzicoOptions);

                if (threedsInitialize.Status == "success")
                {
                    // Redirect to the 3D Secure page
                    // Clean the basket after successful payment
                    if (Request.Cookies["tempUserId"] != null)
                    {
                        var tempUserId = Request.Cookies["tempUserId"];
                        CleanTheBasket(tempUserId, 0, basket);
                    }
                    else
                    {
                        CleanTheBasket(null, user.Id, basket);
                    }
                    return Content(threedsInitialize.HtmlContent, "text/html");
                }
                else
                {
                    sale.SaleStatus = SaleStatus.Unsuccessful;
                    _saleService.TUpdate(sale);
                    // Handle error
                    //ModelState.AddModelError("", "Payment initialization failed.");
                    //return View("Index");
                    return RedirectToAction("PaymentStatus", new { isSuccess = false });
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                //ModelState.AddModelError("", "Payment initialization failed.");
                //return View("Index");
                return RedirectToAction("PaymentStatus", new { isSuccess = false });
            }
        }

        private void CleanTheBasket(string? tempUserId, int userId, Basket basket)
        {
            var basketItems = basket.BasketItems.ToList();
            foreach (var item in basketItems)
            {
                var product = _productService.TGetById(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    _productService.TUpdate(product);
                }

                _basketService.TDeleteBasketItem(tempUserId, userId, item.ProductId, item.Size);
            }
        }

        public IActionResult IyzicoPaymentCallback(IFormCollection iFormCollection)
        {
            // 3ds dan dönen callback verilerini işlenir
            if (iFormCollection["status"] == "success")
            {
                var saleId = iFormCollection["conversationId"];
                var sale = _saleService.TGetById(Convert.ToInt32(saleId));
                sale.SaleStatus = SaleStatus.PaymentApproved;
                _saleService.TUpdate(sale);

                var totalPrice = sale.TotalPrice.ToString();
                var method = sale.PaymentMethod.ToString();

                return RedirectToAction("PaymentStatus", new
                {
                    isSuccess = true,
                    price = totalPrice,
                    paymentMethod = method,
                    transactionId = saleId
                });
            }


            else
                return RedirectToAction("PaymentStatus", new { isSuccess = false });


        }

        [HttpGet]
        public async Task<IActionResult> IyzicoInstallmentCheck(string price, string binNumber) // price kullanılmıyor kaldırılacak
        {
            var user = await _userManager.GetUserAsync(User);

            Basket? basket;
            if (user != null)
                basket = _basketService.TGetBasket(null, user.Id);
            else
                basket = _basketService.TGetBasket(Request.Cookies["tempUserId"], 0);

            //var priceOfBasket = basket.BasketItems.Sum(ci => ci.Product.Price);
            decimal priceOfBasket = 0;

            // Ürünlerin fiyatlarını ve indirimlerini hesapla
            foreach (var item in basket.BasketItems)
            {
                var product = _productService.TGetById(item.ProductId);
                if (product != null)
                {
                    if (product.DiscountRate != null) // product.DiscountEndDate != null && product.DiscountEndDate > DateTime.Now && 
                    {
                        priceOfBasket += (product.Price - (product.Price * (product.DiscountRate ?? 0) / 100)) * item.Quantity;
                    }
                    else if (product.DiscountAmount != null) // product.DiscountEndDate != null && product.DiscountEndDate > DateTime.Now && 
                    {
                        priceOfBasket += (product.Price - product.DiscountAmount.Value) * item.Quantity;
                    }
                    else
                    {
                        priceOfBasket += product.Price * item.Quantity;
                    }
                }
            }

            // Sepetteki kuponları kontrol et ve indirimleri uygula
            foreach (var cc in basket.BasketCoupons)
            {
                var coupon = cc.Coupon;
                if (coupon.DiscountPercentage != null)
                {
                    priceOfBasket = priceOfBasket - (priceOfBasket * coupon.DiscountPercentage.Value / 100);
                }
                else if (coupon.DiscountAmount != null)
                {
                    priceOfBasket -= coupon.DiscountAmount.Value;
                }
            }

            RetrieveInstallmentInfoRequest request = new RetrieveInstallmentInfoRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = "123456789";
            request.BinNumber = binNumber.ToString();
            request.Price = priceOfBasket.ToString();

            InstallmentInfo installmentInfo = await InstallmentInfo.Retrieve(request, _iyzicoOptions);

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
