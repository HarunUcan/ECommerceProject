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
    public class SaleItemManager : ISaleItemService
    {
        private readonly ISaleItemDal _saleItemDal;

        public SaleItemManager(ISaleItemDal saleItemDal)
        {
            _saleItemDal = saleItemDal;
        }

        public void TDelete(SaleItem t)
        {
            _saleItemDal.Delete(t);
        }

        public SaleItem TGetById(int id)
        {
            return _saleItemDal.GetById(id);
        }

        public List<SaleItem> TGetList()
        {
            return _saleItemDal.GetList();
        }

        public void TInsert(SaleItem t)
        {
            _saleItemDal.Insert(t);
        }

        public void TUpdate(SaleItem t)
        {
            _saleItemDal.Update(t);
        }
    }
}
