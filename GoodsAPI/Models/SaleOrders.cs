using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAPI.Models
{
    public class SaleOrders
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string? BPCode { get; set; }

        [Required]
        public DateTime Createdate { get; set; }
        
        public DateTime LastUpdatedate { get; set; }
        
        [Required]
        public int? CreatedBy { get; set; }

        public int? LastUpdatedBy { get; set; }

        [ForeignKey("BPCode")]
        public BusinessPartners? BP { get; set; }

        [ForeignKey("CreatedBy")]
        public Users? Users1 { get; set; }

        [ForeignKey("LastUpdatedBy")]
        public Users? Users2 { get; set; }
    }
}
