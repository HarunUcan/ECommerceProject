using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class PaymentViewModel
    {
        public List<Category>? Categories { get; set; }
        public Basket? Basket { get; set; }
        public List<Adress>? Adresses { get; set; }
    }
}
