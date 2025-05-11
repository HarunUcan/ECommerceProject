using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.SaleDtos;
using ECommerceProject.DtoLayer.Dtos.SaleItemDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _saleService.TGetListWithSaleItems();
            List<SaleDto> saleDtos = new List<SaleDto>();

            foreach(var sale in sales)
            {
                SaleDto saleDto = new SaleDto
                {
                    Id = sale.SaleId,
                    BuyerName = $"{sale.CustomerName} {sale.CustomerSurname}",
                    TotalPrice = sale.TotalPrice,
                    Date = sale.SaleDate,
                    PaymentMethod = sale.PaymentMethod.ToString(),
                    Status = sale.SaleStatus.ToString()
                };

                List<SaleItemDto> itemDtos = new List<SaleItemDto>();
                foreach (var item in sale.SaleItems)
                {
                    SaleItemDto itemDto = new SaleItemDto
                    {
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.SoldUnitPrice
                    };
                    itemDtos.Add(itemDto);
                }
                saleDto.Items = itemDtos;
                saleDtos.Add(saleDto);
            }

            return View(saleDtos);
        }
    }
}
