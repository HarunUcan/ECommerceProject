using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class ProductGroup
    {
        public int ProductGroupId { get; set; }
        public string Name { get; set; } // Ürünlerin isminin başına gelecek olan grup adı
        public string Description { get; set; } // Ürünlere özel açıklama girilmezse bu alan kullanılacak
        public int CategoryId { get; set; } // Ürünlere özel bir kategori seçilmezse bu alan kullanılacak
        public Category Category { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
