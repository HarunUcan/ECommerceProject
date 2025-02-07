using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface IProductImageService : IGenericService<ProductImage>
    {
        Task<List<ProductImage>> SaveProductImageAsync(List<ProductImageDto> productImageDtos);
    }
}
