using ECommerceProject.DtoLayer.Dtos.AdressDtos;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class UserAdressViewModel
    {
        public AdressDto NewAdress { get; set; }
        public List<AdressDto> AdressList { get; set; } = new List<AdressDto>();
    }
}
