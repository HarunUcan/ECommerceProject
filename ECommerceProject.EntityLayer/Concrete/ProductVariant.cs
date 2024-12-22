using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }
        public int Stock { get; set; }
        public ProductSize Size { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
