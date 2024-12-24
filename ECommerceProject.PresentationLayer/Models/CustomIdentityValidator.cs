using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ECommerceProject.PresentationLayer.Models
{
    public class CustomIdentityValidator : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "PasswordTooShort",
                Description = $"Şifre en az {length} karakter olmalıdır."
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = "PasswordMismatch",
                Description = "Şifreler uyuşmuyor."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "DuplicateEmail",
                Description = $"{email} adresi kullanılmaktadır."
            };
        }

        // Bu ugulamada kullanıcı adı mail olarak kullanıldığı için DuplicateUserName metodu üzerinde değişiklik yapıldı.
        public override IdentityError DuplicateUserName(string email)
        {
            return new IdentityError
            {
                Code = "DuplicateUserName",
                Description = $"{email} adresi kullanılmaktadır."
            };
        }
    }
}
