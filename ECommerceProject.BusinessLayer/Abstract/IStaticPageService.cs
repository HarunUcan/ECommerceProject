using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.BusinessLayer.Abstract;

public interface IStaticPageService : IGenericService<StaticPage>
{
    Task<StaticPage> TGetByEnumTypeAsync(StaticPageType staticPageType);
    Task<StaticPageDto> TGetDtoByEnumTypeAsync(StaticPageType staticPageType);
}
