using ECommerceProject.DtoLayer.Dtos.CategoryDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class AdminCategoryEditViewModel
    {
        public CategoryEditDto CategoryEditDto { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
