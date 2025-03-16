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

        public Category TGetBySlug(string slug)
        {
            return _categoryDal.GetBySlug(slug);
        }

        public List<Category> TGetList()
        {
            return _categoryDal.GetList();
        }

        public void TInsert(Category t)
        {
            _categoryDal.Insert(t);
            var categoryWithSlug = _categoryDal.GetList().FirstOrDefault(c => c.CategoryId == t.CategoryId);
            string slug = categoryWithSlug.Name.Aggregate("", (current, c) => current + c.ToString().ToLower());
            slug = slug.Replace(" ", "-");
            slug = slug.Replace("ç", "c");
            slug = slug.Replace("ğ", "g");
            slug = slug.Replace("ı", "i");
            slug = slug.Replace("ö", "o");
            slug = slug.Replace("ş", "s");
            slug = slug.Replace("ü", "u");
            categoryWithSlug.Slug = slug;
            bool isSlugExist = _categoryDal.SearchBySlug(slug);
            if (isSlugExist)
                categoryWithSlug.Slug += $"-{categoryWithSlug.CategoryId}";

            _categoryDal.Update(categoryWithSlug);
        }

        public void TRecursiveDeleteCategory(int categoryId)
        {
            var category = _categoryDal.GetById(categoryId);
            if(category != null && category.ImageUrl != null)
                FileHelper.DeleteFile(category.ImageUrl);

            _categoryDal.RecursiveDeleteCategory(categoryId);
        }

        public async Task<string> TSaveCategoryImageAsync(byte[] imageData, string imageName)
        {
            var filePath = await FileHelper.SaveFileAsync(imageData, $"{Guid.NewGuid()}{Path.GetExtension(imageName)}");
            return filePath;
        }

        public bool TSearchBySlug(string slug)
        {
            return _categoryDal.SearchBySlug(slug);
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
            if(t.ImageUrl != null)
            {
                var category = _categoryDal.GetById(t.CategoryId);
                if (category != null && category.ImageUrl != null)
                    FileHelper.DeleteFile(category.ImageUrl);
            }
            
            _categoryDal.Update(t);
        }
    }
}
