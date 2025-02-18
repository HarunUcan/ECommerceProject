using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class HomeViewModel
    {
        public List<Product>? Products { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
