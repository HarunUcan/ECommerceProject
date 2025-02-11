using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class AdminProductGroupViewModel
    {
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public decimal? GroupPrice { get; set; }
        public string? GroupCurrency { get; set; }
        public int GroupCategoryId { get; set; }
        public List<AdminProductViewModel>? Products { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
