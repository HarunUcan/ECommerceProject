using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public string UniqueCode { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal? DiscountRate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        [NotMapped]
        public decimal? DiscountPrice => (DiscountRate != null && DiscountStartDate <= DateTime.Now && DiscountEndDate >= DateTime.Now) ? Price * (1 - DiscountRate.Value) : 
            (DiscountAmount != null && DiscountStartDate <= DateTime.Now && DiscountEndDate >= DateTime.Now) ? Price - DiscountAmount : Price;
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

    }

    public enum ProductSize
    {
        XS = 0,
        S = 1,
        M = 2,
        L = 3,
        XL = 4,
        XXL = 5
    }
}
