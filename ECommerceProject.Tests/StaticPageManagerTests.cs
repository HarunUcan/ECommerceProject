using ECommerceProject.BusinessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;
using Xunit;

namespace ECommerceProject.Tests;

public class StaticPageManagerTests
{
    [Fact]
    public async Task GetDtoByEnumTypeAsync_ReturnsEmptyDto_WhenPageMissing()
    {
        var manager = new StaticPageManager(new FakeStaticPageDal(null));

        StaticPageDto result = await manager.TGetDtoByEnumTypeAsync(StaticPageType.AboutUs);

        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Title);
        Assert.Equal(string.Empty, result.Content);
        Assert.NotNull(result.UpdatedDate);
    }

    [Fact]
    public async Task GetDtoByEnumTypeAsync_MapsExistingPage()
    {
        var staticPage = new StaticPage
        {
            Id = 5,
            Title = "About",
            Content = "Content",
            UpdatedDate = new DateTime(2024, 1, 1)
        };
        var manager = new StaticPageManager(new FakeStaticPageDal(staticPage));

        StaticPageDto result = await manager.TGetDtoByEnumTypeAsync(StaticPageType.AboutUs);

        Assert.Equal(staticPage.Id, result.Id);
        Assert.Equal(staticPage.Title, result.Title);
        Assert.Equal(staticPage.Content, result.Content);
        Assert.Equal(staticPage.UpdatedDate, result.UpdatedDate);
    }

    private sealed class FakeStaticPageDal : IStaticPageDal
    {
        private readonly StaticPage? _staticPage;

        public FakeStaticPageDal(StaticPage? staticPage)
        {
            _staticPage = staticPage;
        }

        public Task<StaticPage> GetByEnumTypeAsync(StaticPageType staticPageType)
        {
            return Task.FromResult(_staticPage!);
        }

        public void Delete(StaticPage t) => throw new NotImplementedException();

        public StaticPage GetById(int id) => throw new NotImplementedException();

        public List<StaticPage> GetList() => throw new NotImplementedException();

        public void Insert(StaticPage t) => throw new NotImplementedException();

        public void Update(StaticPage t) => throw new NotImplementedException();
    }
}
