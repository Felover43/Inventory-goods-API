using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace GoodsAPI.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class Users
    {
        [Required]
        public int? ID { get; set; }

        [Required]
        [StringLength(1024)]
        public string? FullName { get; set; }

        [Required]
        [StringLength(254)]
      
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public bool? Active { get; set; }

        public ICollection<SaleOrders>? SO1 { get; set; }

        public ICollection<SaleOrders>? SO2 { get; set; }

        public ICollection<PurchaseOrders>? PO1 { get; set; }

        public ICollection<PurchaseOrders>? PO2 { get; set; }
        public Users()
        {
            Active = true;
        }


       // public virtual PurchaseOrdersLines? POL { get; set; }
    }
}
