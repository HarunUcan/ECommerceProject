using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.BusinessLayer.Concrete;

public class StaticPageManager : IStaticPageService
{
    private readonly IStaticPageDal _staticPageDal;

    public StaticPageManager(IStaticPageDal staticPageDal)
    {
        _staticPageDal = staticPageDal;
    }

    public async Task<StaticPage> TGetByEnumTypeAsync(StaticPageType staticPageType)
    {
        return await _staticPageDal.GetByEnumTypeAsync(staticPageType);
    }

    public async Task<StaticPageDto> TGetDtoByEnumTypeAsync(StaticPageType staticPageType)
    {
        var staticPage = await _staticPageDal.GetByEnumTypeAsync(staticPageType);
        if (staticPage == null)
        {
            return new StaticPageDto
            {
                Title = string.Empty,
                Content = string.Empty,
                UpdatedDate = DateTime.Now
            };
        }

        return new StaticPageDto
        {
            Id = staticPage.Id,
            Title = staticPage.Title,
            Content = staticPage.Content,
            UpdatedDate = staticPage.UpdatedDate
        };
    }

    public void TDelete(StaticPage t)
    {
        _staticPageDal.Delete(t);
    }

    public StaticPage TGetById(int id)
    {
        return _staticPageDal.GetById(id);
    }

    public List<StaticPage> TGetList()
    {
        return _staticPageDal.GetList();
    }

    public void TInsert(StaticPage t)
    {
        _staticPageDal.Insert(t);
    }

    public void TUpdate(StaticPage t)
    {
        _staticPageDal.Update(t);
    }
}
