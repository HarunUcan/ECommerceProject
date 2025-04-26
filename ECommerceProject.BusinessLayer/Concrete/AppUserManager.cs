using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
    public class AppUserManager : IAppUserService
    {
        private readonly IAppUserDal _appUserDal;

        public AppUserManager(IAppUserDal appUserDal)
        {
            _appUserDal = appUserDal;
        }

        public void TDelete(AppUser t)
        {
            throw new NotImplementedException();
        }

        public AppUser TGetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<AppUser> TGetList()
        {
            throw new NotImplementedException();
        }

        public async Task<List<AppUser>> TGetUsersBySearchAsync(string searchTerm)
        {
            return await _appUserDal.GetUsersBySearchAsync(searchTerm);
        }

        public void TInsert(AppUser t)
        {
            throw new NotImplementedException();
        }

        public void TUpdate(AppUser t)
        {
            throw new NotImplementedException();
        }
    }
}
