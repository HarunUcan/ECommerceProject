using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DtoLayer.Dtos.ProductImageDtos
{
    public class ProductImageDto
    {
        public byte[] ImageData { get; set; }
        public string ImageName { get; set; }
        public bool IsMain { get; set; }
    }
}
