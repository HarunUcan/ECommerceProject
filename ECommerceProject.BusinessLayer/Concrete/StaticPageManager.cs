using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
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
}
