using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class Adress
    {
        public int AdressId { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string AdressLine { get; set; }
        int UserId { get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
