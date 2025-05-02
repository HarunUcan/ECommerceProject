using ECommerceProject.DtoLayer.Dtos.AdressDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DtoLayer.Dtos.PaymentDtos
{
    public class PaymentDetailDto
    {
        public string? PaymentMethod { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Cvc { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public AdressDto? Adress { get; set; }
    }
}
