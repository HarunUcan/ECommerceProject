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
        public int AdressId { get; set; }
        public Adress Adress { get; set; }
        public List<SaleItem> SaleItems { get; set; }
        public SaleStatus SaleStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }

    public enum SaleStatus
    {
        /// The order is being prepared.
        Preparing = 0,

        /// The order has been shipped.
        Shipped = 1,

        /// The order is in transit to the customer.
        InTransit = 2,

        /// The order has been delivered to the customer.
        Delivered = 3,

        /// The order has been canceled.
        Canceled = 4,

        /// The order has been returned by the customer.
        Returned = 5
    }
}
