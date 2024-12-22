using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public ICollection<Sale> Sales { get; set; }
        public ICollection<Adress> Adresses { get; set; }
        public int? MailConfirmCode { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
