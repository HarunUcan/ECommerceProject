using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;
using ECommerceProject.EntityLayer.Concrete;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
    public class ProductImageManager : IProductImageService
    {
        private readonly IProductImageDal _productImageDal;
        public ProductImageManager(IProductImageDal productImageDal)
        {
            _productImageDal = productImageDal;
        }

        public async Task<List<ProductImage>> SaveProductImageAsync(List<ProductImageDto> productImageDtos)
        {
            List<ProductImage> productImages = new List<ProductImage>();

            foreach (var imageDto in productImageDtos)
            {
                var filePath = await FileHelper.SaveFileAsync(imageDto.ImageData, $"{Guid.NewGuid()}{Path.GetExtension(imageDto.ImageName)}");
                productImages.Add(new ProductImage { Url = filePath, IsMain = imageDto.IsMain });
            }

            return productImages;
        }

        public void TDelete(ProductImage t)
        {
            _productImageDal.Delete(t);
        }

        public ProductImage TGetById(int id)
        {
            return _productImageDal.GetById(id);
        }

        public List<ProductImage> TGetList()
        {
            return _productImageDal.GetList();
        }

        public void TInsert(ProductImage t)
        {
            _productImageDal.Insert(t);
        }

        public void TUpdate(ProductImage t)
        {
            _productImageDal.Update(t);
        }
    }
}
