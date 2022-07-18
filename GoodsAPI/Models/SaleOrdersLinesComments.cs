using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAPI.Models
{
    public class SaleOrdersLinesComments
    {
        [Key]
        [Required]
        public int CommentLineID { get; set; }
        [Required]
        public int DocID { get; set; }
        [Required]
        public int LineID { get; set; }
        [Required]
        public string? Comment { get; set; }

        [Required]
        [ForeignKey("DocID")]
        public SaleOrders? SaleOrders { get; set; }

        [Required]
        [ForeignKey("LineID")]
        public SaleOrdersLines? SaleOrdersLines { get; set; }
    }
}
