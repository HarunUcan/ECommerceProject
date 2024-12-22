using ECommerceProject.BusinessLayer.Abstract;
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
        private readonly ICategoryService _categoryService;

        public CategoryManager(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public void TDelete(Category t)
        {
            _categoryService.TDelete(t);
        }

        public Category TGetById(int id)
        {
            return _categoryService.TGetById(id);
        }

        public List<Category> TGetList()
        {
            return _categoryService.TGetList();
        }

        public void TInsert(Category t)
        {
            _categoryService.TInsert(t);
        }

        public void TUpdate(Category t)
        {
            _categoryService.TUpdate(t);
        }
    }
}
