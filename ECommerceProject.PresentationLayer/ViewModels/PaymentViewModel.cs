using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels;

public class PaymentViewModel
{
    public List<Category>? Categories { get; set; }
    public CartDetailDto? Cart { get; set; }
    public List<Adress>? Adresses { get; set; }
}
