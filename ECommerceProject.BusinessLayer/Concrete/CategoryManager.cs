using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryDal _categoryDal;

        public CategoryManager(ICategoryDal categoryDal)
        {
            _categoryDal = categoryDal;
        }

        public void TDelete(Category t)
        {
            _categoryDal.Delete(t);
        }

        public Category TGetById(int id)
        {
            return _categoryDal.GetById(id);
        }

        public List<Category> TGetList()
        {
            return _categoryDal.GetList();
        }

        public void TInsert(Category t)
        {
            _categoryDal.Insert(t);
        }

        public void TRecursiveDeleteCategory(int categoryId)
        {
            _categoryDal.RecursiveDeleteCategory(categoryId);
        }

        public async Task<string> TSaveCategoryImageAsync(byte[] imageData, string imageName)
        {
            var filePath = await FileHelper.SaveFileAsync(imageData, $"{Guid.NewGuid()}{Path.GetExtension(imageName)}");
            return filePath;
        }

        public bool TToggleFeatured(int categoryId)
        {
            return _categoryDal.ToggleFeatured(categoryId);
        }

        public bool TToggleTopFourCategory(int categoryId)
        {
            return _categoryDal.ToggleTopFourCategory(categoryId);
        }

        public void TUpdate(Category t)
        {
            _categoryDal.Update(t);
        }
    }
}
