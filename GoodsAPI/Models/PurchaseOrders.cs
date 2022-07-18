


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAPI.Models
{
    public class PurchaseOrders
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string? BPCode { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        [Required]
        public int? CreatedBy { get; set; }
        
        public int? LastUpdatedBy { get; set; }

        [ForeignKey("BPCode")]
        public BusinessPartners? BP { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual Users? Users1 { get; set; }

        [ForeignKey("LastUpdatedBy")]
        public virtual Users? Users2 { get; set; }

    }
}
