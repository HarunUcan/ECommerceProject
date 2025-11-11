using ECommerceProject.DtoLayer.Dtos.SaleItemDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DtoLayer.Dtos.SaleDtos
{
    public class SaleDto
    {
        public int Id { get; set; }
        public List<SaleItemDto>? Items { get; set; }
        public string? BuyerName { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
    }
}
