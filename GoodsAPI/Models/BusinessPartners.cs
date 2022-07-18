using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAPI.Models
{
    public class BusinessPartners
    {
        [Key]
        [Required]
        [StringLength(128)]
        public string? BPCode { get; set; }
        
        [Required]
        [StringLength(254)]
        public string? BPName { get; set; }
        [Required]
        [StringLength(1)]
        public char? BPType { get; set; }

        
        public BPType? Btype { get; set; }

       
        [Required]
        public bool Active { get; set; }

        public ICollection<PurchaseOrders>? PO { get; set; }
        public ICollection<SaleOrders>? SO { get; set; }

        public BusinessPartners()
        {
            Active = true;
        }
    }
}
