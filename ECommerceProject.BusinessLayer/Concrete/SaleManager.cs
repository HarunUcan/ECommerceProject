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
    public class SaleManager : ISaleService
    {
        private readonly ISaleDal _saleDal;

        public SaleManager(ISaleDal saleDal)
        {
            _saleDal = saleDal;
        }

        public void TDelete(Sale t)
        {
            _saleDal.Delete(t);
        }

        public Sale TGetById(int id)
        {
            return _saleDal.GetById(id);
        }

        public List<Sale> TGetList()
        {
            return _saleDal.GetList();
        }

        public void TInsert(Sale t)
        {
            _saleDal.Insert(t);
        }

        public void TUpdate(Sale t)
        {
            _saleDal.Update(t);
        }
    }
}
