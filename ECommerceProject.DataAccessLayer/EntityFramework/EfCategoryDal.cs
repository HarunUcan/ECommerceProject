using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Repositories;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.EntityFramework
{
    public class EfCategoryDal : GenericRepository<Category>, ICategoryDal
    {
        public void RecursiveDeleteCategory(int categoryId)
        {
            using var context = new Context();

            // Tüm kategorileri çekiyoruz (tek seferde veritabanına gitmek için)
            // Eğer tüm kategorileri çekmezsek çocukların alt kategorileri null dönüyor
            // Bu işlemi derinliği bilemediğimiz için yapıyoruz
            var allCategories = context.Categories
                .Include(c => c.SubCategories)
                .ToList();

            var category = allCategories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null) return;

            List<Category> categoriesToRemove = new List<Category>();
            GetAllSubCategories(category, categoriesToRemove);

            // 1️) Önce ParentCategory bağlantısını kopar (same table hatasını önler)
            foreach (var cat in categoriesToRemove)
            {
                cat.ParentCategory = null;
            }
            context.SaveChanges(); // Güncellemeyi kaydet

            // 2️) Şimdi kategorileri silebiliriz
            context.Categories.RemoveRange(categoriesToRemove);
            context.SaveChanges();
        }

        private static void GetAllSubCategories(Category category, List<Category> categoriesToRemove)
        {
            foreach (var subCategory in category.SubCategories)
            {
                GetAllSubCategories(subCategory, categoriesToRemove);
                categoriesToRemove.Add(subCategory);
            }
            categoriesToRemove.Add(category); // Ana kategoriyi de ekleyelim
        }

        public bool ToggleFeatured(int categoryId)
        {
            using var context = new Context();
            var category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null) return false;

            category.IsFeatured = !category.IsFeatured;
            context.SaveChanges();
            return true;
        }

        public bool ToggleTopFourCategory(int categoryId)
        {
            using var context = new Context();
            var category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null) return false;

            category.IsTopFourCategory = !category.IsTopFourCategory;
            context.SaveChanges();
            return true;
        }

        public bool SearchBySlug(string slug)
        {
            using var context = new Context();
            return context.Categories.Any(c => c.Slug == slug);
        }

        public Category GetBySlug(string slug)
        {
            using var context = new Context();
            return context.Categories.FirstOrDefault(c => c.Slug == slug);
        }
    }
}
