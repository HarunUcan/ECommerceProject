using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface IMailSenderService
    {
        Task SendConfirmationMailAsync(string mail,string subject, string mailBody);
    }
}
