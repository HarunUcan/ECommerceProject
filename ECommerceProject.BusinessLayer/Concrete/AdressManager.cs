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
    public class AdressManager : IAdressService
    {
        private readonly IAdressDal _adressDal;

        public AdressManager(IAdressDal adressDal)
        {
            _adressDal = adressDal;
        }

        public void TDelete(Adress t)
        {
            _adressDal.Delete(t);
        }

        public Adress TGetById(int id)
        {
            return _adressDal.GetById(id);
        }

        public List<Adress> TGetList()
        {
            return _adressDal.GetList();
        }

        public void TInsert(Adress t)
        {
            _adressDal.Insert(t);
        }

        public void TUpdate(Adress t)
        {
            _adressDal.Update(t);
        }
    }
}
