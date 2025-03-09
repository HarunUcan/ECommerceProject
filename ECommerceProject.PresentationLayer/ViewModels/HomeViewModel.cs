using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class HomeViewModel
    {
        public List<Product>? FeaturedProducts { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Product>? FeaturedCategoryProducts { get; set; }
    }
}
