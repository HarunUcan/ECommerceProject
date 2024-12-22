using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class SaleItem
    {
        public int SaleItemId { get; set; }
        public int Quantity { get; set; }
        public ProductSize Size { get; set; }
        public decimal SoldUnitPrice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
    }
}
