using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DtoLayer.Dtos.CategoryDtos
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
