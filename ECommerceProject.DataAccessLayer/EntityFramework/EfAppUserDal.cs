using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Repositories;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.EntityFramework
{
    public class EfAppUserDal : GenericRepository<AppUser>, IAppUserDal
    {
        public async Task<List<AppUser>> GetUsersBySearchAsync(string searchTerm)
        {
            using var context = new Context();

            return await context.Users
                .Where(u => u.Name.Contains(searchTerm) || u.Surname.Contains(searchTerm) || u.Email.Contains(searchTerm))
                .Select(u => new AppUser
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email
                })
                .ToListAsync();
        }
    }
}
