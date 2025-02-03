using ECommerceProject.DtoLayer.Dtos.CategoryDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class AdminCategoryViewModel
    {
        public CategoryDto CategoryDto { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
