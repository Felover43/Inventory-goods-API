using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAPI.Models
{
    public class SaleOrdersLines
    {
        [Key]
        [Required]
        public int LineID { get; set; }

        [Required]
        public int DocID { get; set; }

        [Required]
        [StringLength(128)]
        public string? ItemCode { get; set; }

        [Required]
       [Column(TypeName = "decimal(38, 18)")]
        public decimal? Quantity { get; set; }


        [ForeignKey("DocID")]
        public SaleOrders? SaleOrders { get; set; }


        [ForeignKey("ItemCode")]
        public Items? items { get; set; }

    }
}
