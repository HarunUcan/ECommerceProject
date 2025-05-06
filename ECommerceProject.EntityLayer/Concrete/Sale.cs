using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class Sale
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public string? CustomerName { get; set; } // Müşteri adı
        public string? CustomerSurname { get; set; } // Müşteri soyadı
        public string? CustomerEmail { get; set; } // Müşteri e-posta adresi
        public string? CustomerPhone { get; set; } // Müşteri telefon numarası
        public int AdressId { get; set; }
        public Adress Adress { get; set; }
        public List<SaleItem> SaleItems { get; set; }
        public SaleStatus SaleStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? TempCartId { get; set; } // Eğer kullanıcı giriş yapmamışsa, AppUserId yerine geçici sepet ID'si (cookie) kullanılacak.
        public decimal TotalPrice { get; set; }
        public int InstallmentCount { get; set; } // Taksit sayısı
        public PaymentMethod? PaymentMethod { get; set; } // Ödeme yöntemi (Kredi kartı, havale, kapıda ödeme vb.)
    }

    public enum SaleStatus
    {
        /// The order is not approved.
        NotApproved = 0,

        /// The order is approved and the payment is successful.
        PaymentApproved = 1,

        /// The order is being prepared.
        Preparing = 2,

        /// The order has been shipped.
        Shipped = 3,

        /// The order is in transit to the customer.
        InTransit = 4,

        /// The order has been delivered to the customer.
        Delivered = 5,

        /// The order has been canceled.
        Canceled = 6,

        /// The order has been returned by the customer.
        Returned = 7,

        /// The order was unsuccessful due to payment failure or other issues.
        Unsuccessful = 8,
    }

    public enum PaymentMethod
    {
        /// The payment was made by credit card.
        CreditCard = 0,

        /// The payment was made by bank transfer.
        BankTransfer = 1,

        /// The payment was made by cash on delivery.
        CashOnDelivery = 2,
    }
}
