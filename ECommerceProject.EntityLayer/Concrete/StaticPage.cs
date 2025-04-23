using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class StaticPage
    {
        public int Id { get; set; }
        public StaticPageType StaticPageType { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public enum StaticPageType
    {
        PrivacyPolicy,
        CookiePolicy,
        DistanceSalesAgreement,
        ReturnAndRefundPolicy,
        MembershipAgreement,
        KVKK,
        AboutUs,
        Store,
        FAQ,
        PaymentOptions,
        TermsOfUse
    }
}

