using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface ICategoryService : IGenericService<Category>
    {
        void TRecursiveDeleteCategory(int categoryId);
        bool TToggleFeatured(int categoryId);
        bool TToggleTopFourCategory(int categoryId);
        Task<string> TSaveCategoryImageAsync(byte[] imageData, string imageName);
    }
}
